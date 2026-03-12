using CommonObjects.DTO;
using CommonObjects.Requests.Attachments;
using CommonObjects.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.ServiceRealizations.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.Setters;
using System.Collections.Concurrent;
using System.Collections.ObjectModel;

namespace SpoofMess.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly CancellationTokenSource token = new();
    private readonly IChatApiService _chatApiService;
    private readonly IChatUserApiService _chatUserApiService;
    private readonly IMessageApiService _messageApiService;
    private readonly IMessageService _messageService;
    private readonly INotificationService _notificationService;
    private readonly IFileApiService _fileApiService;
    private readonly IAttachmentService _attachmentService;
    private readonly IFingerprintService _fingerprintService;

    public ObservableCollection<Chat> Chats { get; set; } = [];
    private ConcurrentDictionary<Guid, User> Users { get; set; } = [];
    [ObservableProperty]
    private Chat? _selectedChat;

    public MainViewModel(
        IChatApiService chatApiService,
        IMessageService messageService,
        IChatUserApiService chatUserService,
        IMessageApiService messageApiService,
        INotificationService notificationService,
        IFileApiService fileApiService,
        IAttachmentService attachmentService,
        IFingerprintService fingerprintService)
    {
        _chatApiService = chatApiService;
        _messageService = messageService;
        _chatUserApiService = chatUserService;
        _notificationService = notificationService;
        _attachmentService = attachmentService;
        _messageApiService = messageApiService;
        _messageService.OnMessageReceived += OnMessageReceived;
        _fileApiService = fileApiService;
        _fingerprintService = fingerprintService;
        //It's so bad...
        LoadSkippedData();
    }

    private async void LoadSkippedData()
    {
        await LoadChats();
        await LoadMessages();
    }

    private async Task LoadChats()
    {
        Result<List<ChatUserDTO>> chats = await _chatUserApiService.GetChats(DateTime.UtcNow.AddMonths(-10));
        if (chats.Success)
        {
            foreach (ChatUserDTO chat in chats.Body!)
                Chats.Add(chat.Set());
        }
    }
    private async Task LoadMessages()
    {
        Result<List<MessageDTO>> chats = await _messageApiService.GetSkippedMessages(DateTime.UtcNow.AddMonths(-10));
        if (chats.Success)
        {
            MessageModel message;
            Chat? chat;
            Result<ChatDTO> result;
            foreach (MessageDTO messageDTO in chats.Body!)
            {
                message = messageDTO.Set();
                chat = Chats.FirstOrDefault(x => x.Id == message.ChatId);
                if (chat is null)
                {
                    result = await _chatApiService.GetChat(message.ChatId);
                    if (!result.Success)
                        continue;
                    chat = result.Body!.Set();
                }
                chat.Messages.Add(message);
                message.Chat = chat;
            }
        }
    }

    private async Task LoadUser(Guid userId, MessageModel message)
    {
        if (Users.TryGetValue(userId, out User? user))
            message.User = user;
        else
        {

        }
    }

    private async void OnMessageReceived(MessageModel obj)
    {
        Chat? chat = Chats.FirstOrDefault(x => x.Id == obj.ChatId);
        if (chat is null)
        {
            Result<ChatDTO> result = await _chatApiService.GetChat(obj.ChatId);
            if (result.Success)
                chat = result.Body!.Set();
            else
            {
                _notificationService.ShowToast(new()
                {
                    Text = (string.IsNullOrEmpty(result.Error) ? result.Message : result.Error) ?? "Unexpected",
                    Type = Enums.NotificationType.Fail
                });
                return;
            }
        }
        //Need to check sender
        App.Current.Dispatcher.Invoke(() =>
        {
            chat.Messages.Add(obj);
        });
    }

    [RelayCommand]
    private async Task Send()
    {
        if (SelectedChat is null) return;
        Chat currentChat = SelectedChat;
        MessageModel request = SelectedChat.CurrentMessage;
        request.ChatId = SelectedChat.Id;
        currentChat.CurrentMessage = new()
        {
            ChatId = SelectedChat.Id,
            Text = string.Empty
        };
        List<Attachment> attachments = [];
        await Parallel.ForEachAsync(request.Attachments, async (file, cancelationToken) =>
        {
            Result<byte[]> result = await _attachmentService.SendAttachment(file);
            if (!result.Success)
                for (int i = 0; i < 5; i++)
                {
                    result = await _attachmentService.SendAttachment(file);
                    if (result.Success)
                    {
                        attachments.Add(new(result.Body!, file.Name!, file.Size));
                    }
                }
            else
                attachments.Add(new(result.Body!, file.Name!, file.Size));
        });
        await _messageService.SendMessage(request.Set(attachments));
    }
    [RelayCommand]
    private void Attach()
    {
        if (SelectedChat is null) return;

        _attachmentService.Attach(SelectedChat.CurrentMessage);
    }

    [RelayCommand]
    private void Unattach(FileObject fileObject)
    {

    }
    public void Dispose()
    {
        token.Cancel();
        GC.SuppressFinalize(this);
    }
}