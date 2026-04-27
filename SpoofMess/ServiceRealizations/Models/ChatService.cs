using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using CommonObjects.Results;
using SpoofFileParser.FileMetadata;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.Setters;
using System.Collections.ObjectModel;
using System.IO;

namespace SpoofMess.ServiceRealizations.Models;

public class ChatService(IChatApiService chatApiService, IFileService fileService, UserInfo userInfo, ISerializer serializer) : IChatService
{
    public ObservableCollection<Chat> Chats { get; set; } = [];

    private readonly UserInfo _userInfo = userInfo;
    private readonly IChatApiService _chatApiService = chatApiService;
    private readonly ISerializer _serializer = serializer;
    private readonly IFileService _fileService = fileService;

    public async Task<Chat?> Get(Guid id)
    {
        Chat? chat = Chats.FirstOrDefault(c => c.Id == id);
        if (chat is not null)
            return chat;

        Result<ChatDTO> chatResult = await _chatApiService.GetChat(id);
        if (!chatResult.Success)
            return null;
        chat = chatResult.Body!.Set();
        Chats.Add(chat);
        return chat;
    }

    public async Task AddChats(List<ChatUserDTO> chats)
    {
        Chat newChat;
        foreach (ChatUserDTO chat in chats)
        {
            newChat = chat.Set();
            Chats.Add(newChat);
            if(chat.ChatAvatarToken is not null)
            {
                FileObject file = new()
                {
                    Id = chat.ChatAvatarId,
                    Token = chat.ChatAvatarToken,
                    Category = Enums.FileCategory.Image,
                    Name = chat.OriginalFileName,
                    Path = Path.Combine(_userInfo.SessionSettings.Directory, chat.OriginalFileName),
                    Metadata = _serializer.Deserialize<IFileMetadata>(chat.Metadata ?? "")
                };
                file.Size = file.Metadata.Size;
                file.PrettySize = _fileService.ToPrettySize(file.Metadata.Size);
                await _fileService.Save(file);
                newChat.Avatar = file;
            }
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
        if(result.Success)
            Chats.Add(result.Body!.Set());
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
}
