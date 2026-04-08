using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Collections.ObjectModel;

namespace SpoofMess.ViewModels;

public partial class MainViewModel : ObservableObject, IDisposable
{
    private readonly CancellationTokenSource token = new();
    private readonly IMessageService _messageService;
    private readonly INotificationApiService _notificationApiService;
    private readonly INotificationService _notificationService;
    private readonly IAttachmentService _attachmentService;
    private readonly IChatUserService _chatUserService;
    private readonly IChatService _chatService;
    private readonly INavigationService _navigationService;

    [ObservableProperty]
    private Chat? _selectedChat;
    [ObservableProperty]
    private object? _additionalView;
    public ObservableCollection<Chat> Chats { get; set; }

    public MainViewModel(
        INotificationService notificationService,
        INotificationApiService notificationApiService,
        IMessageService messageService,
        IAttachmentService attachmentService,
        IChatUserService chatUserService,
        INavigationService navigationService,
        IChatService chatService)
    {
        _notificationService = notificationService;
        _notificationApiService = notificationApiService;
        _chatUserService = chatUserService;
        _attachmentService = attachmentService;
        _messageService = messageService;
        _navigationService = navigationService;
        _notificationApiService.OnMessageReceived += _messageService.UploadMessage;
        _notificationApiService.OnMessageEdited += _messageService.EditHandle;
        //It's so bad...
        LoadSkippedData();
        ServiceRealizations.EventHandler.OnDelete += _messageService.DeleteLocal;
        ServiceRealizations.EventHandler.OnEdit += _messageService.StartEdit;
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
            _notificationService.ShowToast(new() { Text = ex.Message, Type = Enums.NotificationType.Error });
        }
    }


    [RelayCommand]
    private async Task Send()
    {
        try
        {
            await _messageService.SendMessage(SelectedChat);
        }
        catch (Exception ex)
        {
            _notificationService.ShowToast(new() { Text = ex.Message, Type = Enums.NotificationType.Error });
        }
    }

    [RelayCommand]
    private void Attach()
    {
        if (SelectedChat is null) return;

        _attachmentService.Attach(SelectedChat.CurrentMessage);
    }

    [RelayCommand]
    private void ShowCreateGroup()
    {
        AdditionalView = _navigationService.GetCreateGroupViewModel();
    }

    [RelayCommand]
    private void Unattach(FileObject fileObject)
    {
        if (SelectedChat is null) return;
        _attachmentService.Unattach(fileObject, SelectedChat.CurrentMessage);
    }

    [RelayCommand]
    private void ShowSettings()
    {
        AdditionalView = _navigationService.GetSettingsViewModel();
    }

    [RelayCommand]
    private async Task StopEdit(MessageModel message)
    {
        await _messageService.StopEdit(message, SelectedChat);
    }

    public void Dispose()
    {
        token.Cancel();
        GC.SuppressFinalize(this);
    }
}