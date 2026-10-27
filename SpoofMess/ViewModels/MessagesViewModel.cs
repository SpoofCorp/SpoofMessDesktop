using CommonObjects.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.ServiceRealizations;
using SpoofMess.Services;
using SpoofMess.Services.Models;
using System.Windows;

namespace SpoofMess.ViewModels;

public partial class MessagesViewModel(
    IMessageService messageService,
    INotificationService notificationService,
    IAttachmentService attachmentService,
    FileAudioRecorderService fileAudioRecorderService) : BaseViewModel
{
    protected readonly INotificationService _notificationService = notificationService;
    protected readonly IAttachmentService _attachmentService = attachmentService;
    protected readonly FileAudioRecorderService _fileAudioRecorderService = fileAudioRecorderService;
    protected readonly IMessageService _messageService = messageService;
    [ObservableProperty]
    public partial bool IsVoiceRecording { get; set; }
    private Action<Chat> getChatInfo = null!;
    public Visibility SelectChatVisibility => SelectedChat is null ? Visibility.Visible : Visibility.Collapsed;
    public Visibility ChatMessagesVisibility => SelectedChat is null ? Visibility.Collapsed : Visibility.Visible;

    [NotifyPropertyChangedFor(nameof(ChatMessagesVisibility))]
    [NotifyPropertyChangedFor(nameof(SelectChatVisibility))]
    [ObservableProperty]
    public partial Chat? SelectedChat { get; set; }

    [ObservableProperty]
    public partial bool IsOpen { get; set; } = false;

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
    private async Task ScrollToTop()
    {
        _notificationService.ShowError("You scroll to top :3");
    }

    [RelayCommand]
    private void Attach()
    {
        if (SelectedChat is null) return;

        _attachmentService.Attach(SelectedChat.CurrentMessage);
    }

    [RelayCommand]
    private async Task StopVoiceRecord()
    {
        await _fileAudioRecorderService.Stop();
        IsVoiceRecording = false;
    }

    [RelayCommand]
    private async Task VoiceRecord()
    {
        if (SelectedChat is null) return;
        IsVoiceRecording = true;
        Result<FileObject> result = await _fileAudioRecorderService.RecordToFile();
        if (!result)
            return;

        _attachmentService.AttachVoice(SelectedChat.CurrentMessage, result.Body!);
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
            getChatInfo(SelectedChat);
    }

    protected override async Task InitializeAsync(bool initialize)
    {
        _fileAudioRecorderService.Initialize();
        await base.InitializeAsync(initialize);
    }

    public void SetGetChatInfo(Action<Chat> func) =>
        getChatInfo = func;

    public void OnSelectionChatChanged(Chat? chat) =>
        SelectedChat = chat;

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        SelectedChat?.IsOpened = true;
        await base.DisposeAsync(disposing);
    }
}
