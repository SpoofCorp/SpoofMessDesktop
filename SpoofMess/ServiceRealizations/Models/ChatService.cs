using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofFileParser.FileMetadata;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.Setters;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;
using System.IO;

namespace SpoofMess.ServiceRealizations.Models;

public class ChatService(IChatApiService chatApiService, IChatAvatarService chatAvatarService, IFileService fileService, UserInfo userInfo, ISerializer serializer) : IChatService
{
    public ObservableCollection<Chat> Chats { get; set; } = [];
    public ConcurrentDictionary<Guid, Chat> ChatsStorage { get; set; } = [];

    private readonly UserInfo _userInfo = userInfo;
    private readonly IChatApiService _chatApiService = chatApiService;
    private readonly IChatAvatarService _chatAvatarService = chatAvatarService;
    private readonly ISerializer _serializer = serializer;
    private readonly IFileService _fileService = fileService;

    public event Action<Chat, int?> ChatAdded = null!;

    public event Action<int, int> ChatMoved = null!;
    private readonly SemaphoreSlim _chatLock = new(1, 1);


    public async Task<Chat?> Get(Guid id, bool saveToLocal = true)
    {
        if (ChatsStorage.TryGetValue(id, out Chat? chat))
        {
            return chat;
        }

        await _chatLock.WaitAsync();
        try
        {
            if (ChatsStorage.TryGetValue(id, out chat))
            {
                return chat;
            }

            Result<ChatDTO> chatResult = await _chatApiService.GetChat(id);
            if (!chatResult || chatResult.Body is null)
                return null;

            chat = chatResult.Body.Set();

            if (ChatsStorage.TryAdd(id, chat))
            {
                if (saveToLocal)
                {
                    Add(chat);
                }
            }

            await UploadAvatar(chat, chatResult.Body);
            return chat;
        }
        finally
        {
            _chatLock.Release();
        }
    }

    private async Task UploadAvatar(Chat chat, ChatDTO chatDTO)
    {
        if (chatDTO.AvatarToken is null) return;

        chat.Avatar = new()
        {
            Token = chatDTO.AvatarToken,
            Id = chatDTO.AvatarId,
            Name = chatDTO.OriginalAvatarFileName,
            Path = Path.Combine(_userInfo.SessionSettings.Directory, chatDTO.OriginalAvatarFileName ?? chatDTO.UniqueName)
        };

        await _chatAvatarService.UploadAvatar(chat);
    }

    public async Task AddChats(List<ChatUserDTO> chats)
    {
        await Parallel.ForEachAsync(chats, async (chat, cancellationToken) =>
        {
            Chat newChat = chat.Set();
            Add(newChat);
            if (!ChatsStorage.TryAdd(newChat.Id, newChat) || chat.AvatarFileToken is null)
                return;
            FileObject file = new()
            {
                Id = chat.AvatarId,
                Token = chat.AvatarFileToken,
                AttachmentToken = chat.AvatarAccessToken,
                Category = Enums.FileCategory.Image,
                Name = chat.OriginalFileName,
                Path = Path.Combine(_userInfo.SessionSettings.Directory, chat.OriginalFileName ?? ""),
                Metadata = chat.Metadata is null ? new ImageMetadata() : _serializer.Deserialize<IFileMetadata>(chat.Metadata)
            };
            file.Size = file.Metadata.Size;
            file.PrettySize = _fileService.ToPrettySize(file.Metadata.Size);
            await _fileService.Save(file, cancellationToken);
            newChat.Avatar = file;
        });
    }

    private void Add(Chat newChat)
    {
        Chat? olderChat = null;
        DateTime timeNewChat = newChat.LastMessage is null ? newChat.CreatedAt : newChat.LastMessage.SentAt;

        lock (Chats)
        {
            if (Chats.Any(x => x.UniqueName == newChat.UniqueName))
                return;
            foreach (Chat chat in Chats)
            {
                if (chat.LastMessage is null ? chat.CreatedAt <= timeNewChat : chat.LastMessage.SentAt <= timeNewChat)
                {
                    olderChat = chat;
                    break;
                }
            }
            int? index = null;
            if (olderChat is null)
                Chats.Add(newChat);
            else
            {
                index = Chats.IndexOf(olderChat);
                Chats.Insert(index!.Value, newChat);
            }
            ChatAdded?.Invoke(newChat, index);
        }
    }

    public async Task CreateChat(Chat chat)
    {
        Result<ChatDTO> result = await _chatApiService.Create(new()
        {
            ChatName = chat.Name ?? string.Empty,
            UniqueName = chat.UniqueName,
            ChatTypeId = chat.ChatTypeId,
            IsPublic = chat.IsPublic
        });
        if (result.Success)
            Add(result.Body!.Set());
    }

    public async void Update(ChangeChatSettingsRequest request)
    {
        Chat? chat = await Get(request.Id);
        if (chat is null)
            return;
        chat.IsPublic = request.IsPublic ?? chat.IsPublic;
        chat.UniqueName = request.UniqueName ?? chat.UniqueName;
        chat.Name = request.ChatName ?? chat.Name;
    }

    public async void OnMessageAdded(MessageModel message)
    {
        Chat? currentChat = await Get(message.ChatId);
        if (currentChat is null)
            return;
        DateTime timeCurrentChat = currentChat.LastMessage is null ? currentChat.CreatedAt : currentChat.LastMessage.SentAt;
        Chat? olderChat;
        lock (Chats)
            olderChat = Chats.FirstOrDefault(x => x.LastMessage is null ? x.CreatedAt <= timeCurrentChat : x.LastMessage.SentAt <= timeCurrentChat);
        if (olderChat is null)
            return;
        else
        {
            lock (Chats)
            {
                int current = Chats.IndexOf(currentChat), old = Chats.IndexOf(olderChat);
                if (current == -1)
                    return;
                Chats.Move(current, old);
                ChatMoved(current, old);
            }
        }
    }

    public async void OnChatAvatarUpdated(ChatAvatarResponse response)
    {
        Chat? chat = await Get(response.ChatId);
        if (chat is null)
            return;
        FileObject avatar = new()
        {
            AttachmentToken = response.AvatarTokenAccess
        };
        chat.Avatar = avatar;
        chat.Avatars.Add(avatar);
        await _chatAvatarService.OnChatAvatarUpdated(avatar);
    }
}
