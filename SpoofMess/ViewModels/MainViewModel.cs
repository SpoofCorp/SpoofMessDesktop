using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;

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
    public ObservableCollection<Chat> Chats { get; set; }

    [NotifyPropertyChangedFor(nameof(AdditionalVisibility))]
    [ObservableProperty]
    private AdditionalViewModel? _additionalView;

    [NotifyPropertyChangedFor(nameof(SelectChatVisibility))]
    [ObservableProperty]
    private Chat? _selectedChat;
    private readonly UserInfo _userInfo;
    [ObservableProperty]
    private Visibility _sideMenuVisibility = Visibility.Collapsed; 
    public Visibility AdditionalVisibility => AdditionalView is null ? Visibility.Collapsed : Visibility.Visible;

    public Visibility SelectChatVisibility => SelectedChat is null ? Visibility.Visible : Visibility.Collapsed;

    public MainViewModel() { }

    public MainViewModel(
        INotificationService notificationService,
        INotificationApiService notificationApiService,
        IMessageService messageService,
        IAttachmentService attachmentService,
        IChatUserService chatUserService,
        INavigationService navigationService,
        IChatService chatService,
        UserInfo userInfo)
    {
        _notificationService = notificationService;
        _notificationApiService = notificationApiService;
        _chatUserService = chatUserService;
        _attachmentService = attachmentService;
        _messageService = messageService;
        _navigationService = navigationService;
        _notificationApiService.OnMessageReceived += _messageService.UploadMessage;
        _notificationApiService.OnMessageEdited += _messageService.EditHandle;
        ServiceRealizations.EventHandler.OnDelete += _messageService.DeleteLocal;
        ServiceRealizations.EventHandler.OnEdit += _messageService.StartEdit;
        _chatService = chatService;
        Chats = _chatService.Chats;
        _userInfo = userInfo;
        //It's so bad...
        LoadSkippedData();
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
    private void ShowProfile()
    {
        AdditionalView = _navigationService.GetProfileViewModel(this, Close);
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
        AdditionalView = _navigationService.GetCreateGroupViewModel(this, Close);
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
        AdditionalView = _navigationService.GetSettingsViewModel(this, Close);
    }

    [RelayCommand]
    private async Task StopEdit(MessageModel message)
    {
        await _messageService.StopEdit(message, SelectedChat);
    }

    private void Close() =>
        AdditionalView = null; 

    public void Dispose()
    {
        token.Cancel();
        GC.SuppressFinalize(this);
    }

    public void Close(Window window, CancelEventArgs e)
    {
        if (_userInfo.HideToTray)
        {
            window.Hide();
            if(_userInfo.UnselectChat)
                SelectedChat = null;
            AdditionalView?.CloseCommand.Execute(null);
            AdditionalView = null;
            e.Cancel = true;
            SideMenuVisibility = Visibility.Collapsed;
        }
    }
}