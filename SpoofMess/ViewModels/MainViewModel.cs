using CommonObjects.DTO;
using CommonObjects.Requests.Attachments;
using CommonObjects.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.Setters;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly CancellationTokenSource token = new();
    private readonly IMessageService _messageService;
    private readonly INotificationApiService _notificationApiService;
    private readonly IAttachmentService _attachmentService;
    private readonly IChatUserService _chatUserService;
    private readonly IChatService _chatService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Chat? _selectedChat;
    public ObservableCollection<Chat> Chats { get; set; }

    public MainViewModel(
        INotificationApiService notificationApiService,
        IMessageService messageService,
        IAttachmentService attachmentService,
        IChatUserService chatUserService,
        INavigationService navigationService,
        IChatService chatService)
    {
        _notificationApiService = notificationApiService;
        _chatUserService = chatUserService;
        _attachmentService = attachmentService;
        _messageService = messageService;
        _navigationService = navigationService;
        _notificationApiService.OnMessageReceived += OnMessageReceived;
        //It's so bad...
        LoadSkippedData();
        _chatService = chatService;
        Chats = _chatService.Chats;
    }

    public async void LoadSkippedData()
    {
        try
        {
            await _chatUserService.SyncChats(DateTime.UtcNow.AddMonths(-10));
            await _messageService.LoadSkippedMesssages(DateTime.UtcNow.AddMonths(-10));
        }
        catch (Exception ex)
        {
            MessageBox.Show(ex.Message);
        }
    }

    private async void OnMessageReceived(MessageDTO obj)
    {
        await _messageService.UploadMessage(obj);
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
            Result<byte[]> result = await _attachmentService.SendAttachment(file.File);
            if (!result.Success)
                for (int i = 0; i < 5; i++)
                {
                    result = await _attachmentService.SendAttachment(file.File);
                    if (result.Success)
                    {
                        attachments.Add(new(result.Body!, file.File.Name!, string.Empty, file.File.Size));
                        break;
                    }
                }
            else
                attachments.Add(new(result.Body!, file.File.Name!, string.Empty, file.File.Size));
        });
        await _notificationApiService.SendMessage(request.Set(attachments));
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

    [RelayCommand]
    private void ShowSettings()
    {
        
    }

    public void Dispose()
    {
        token.Cancel();
        GC.SuppressFinalize(this);
    }
}