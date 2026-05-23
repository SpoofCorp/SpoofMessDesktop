using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class MessagesViewModel(
    IMessageService messageService,
    INotificationService notificationService,
    IAttachmentService attachmentService,
    INotificationApiService notificationApiService) : ObservableObject, IInitializable, IDisposable
{
    private readonly INotificationService _notificationService = notificationService;
    private readonly IAttachmentService _attachmentService = attachmentService;
    private readonly IMessageService _messageService = messageService;
    private readonly INotificationApiService _notificationApiService = notificationApiService;
    private Action getChatInfo = null!; 
    public Visibility SelectChatVisibility => SelectedChat is null ? Visibility.Visible : Visibility.Collapsed;

    [NotifyPropertyChangedFor(nameof(SelectChatVisibility))]
    [ObservableProperty]
    private Chat? _selectedChat;
    [ObservableProperty]
    public bool _isOpen = false;

    [RelayCommand]
    private async Task Send()
    {
        try
        {
            await _messageService.SendMessage(SelectedChat);
        }
        catch (Exception ex)
        {
            _notificationService.ShowError(ex.Message);
        }
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
        if (SelectedChat is null) return;
        _attachmentService.Unattach(fileObject, SelectedChat.CurrentMessage);
    }

    [RelayCommand]
    private async Task StopEdit(MessageModel message)
    {
        await _messageService.StopEdit(SelectedChat);
    }

    [RelayCommand]
    private void ShowChatInfo()
    {
        IsOpen = false;
        if (SelectedChat is not null)
            getChatInfo();
    }

    public async void InitializeAsync()
    {
        _notificationApiService.OnMessageReceived += _messageService.UploadMessage;
        _notificationApiService.OnMessageEdited += _messageService.EditHandle;
        _notificationApiService.OnMessageDeleted += _messageService.OnDelete;
        ServiceRealizations.EventHandler.OnDelete += _messageService.DeleteLocal;
        ServiceRealizations.EventHandler.OnEdit += _messageService.StartEdit;
        await _messageService.LoadSkippedMesssages(DateTime.UtcNow.AddMonths(-10));
    }

    public void SetGetChatInfo(Action func) =>
        getChatInfo = func;

    public void OnSelectionChatChanged(Chat? chat) =>
        SelectedChat = chat;

    public void Dispose()
    {
        _notificationApiService.OnMessageReceived -= _messageService.UploadMessage;
        _notificationApiService.OnMessageDeleted -= _messageService.OnDelete;
        _notificationApiService.OnMessageEdited -= _messageService.EditHandle;
        ServiceRealizations.EventHandler.OnDelete -= _messageService.DeleteLocal;
        ServiceRealizations.EventHandler.OnEdit -= _messageService.StartEdit;
        GC.SuppressFinalize(this);
    }
}
