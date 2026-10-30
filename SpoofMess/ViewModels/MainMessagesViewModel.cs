using SpoofMess.ServiceRealizations;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.ViewModels.Notifications;

namespace SpoofMess.ViewModels;

public partial class MainMessagesViewModel(
    IMessageService messageService,
    INotificationService notificationService,
    IAttachmentService attachmentService,
    FileAudioRecorderService fileAudioRecorderService,
    INotificationApiService notificationApiService,
    StaticNotificationViewModel staticNotificationViewModel,
    IAppLifeCycleService appLifeCycleService) : MessagesViewModel(
        messageService,
        notificationService,
        attachmentService,
        fileAudioRecorderService,
        staticNotificationViewModel,
        appLifeCycleService)
{
    private readonly INotificationApiService _notificationApiService = notificationApiService;

    protected override async Task InitializeAsync(bool initialize)
    {
        await StaticNotificationViewModel.InitializeAsync();
        _notificationApiService.OnMessageReceived += _messageService.UploadMessage;
        _notificationApiService.OnMessageEdited += _messageService.EditHandle;
        _notificationApiService.OnMessageDeleted += _messageService.OnDelete;
        ServiceRealizations.EventHandler.OnDelete += _messageService.DeleteLocal;
        ServiceRealizations.EventHandler.OnEdit += _messageService.StartEdit;
        await _messageService.LoadSkippedMesssages(
            DateTime.UtcNow.AddMonths(-10),
            _appLifeCycleService.AppCancellationToken
        );
        await base.InitializeAsync(initialize);
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        if (disposing)
        {
            _notificationApiService.OnMessageReceived -= _messageService.UploadMessage;
            _notificationApiService.OnMessageDeleted -= _messageService.OnDelete;
            _notificationApiService.OnMessageEdited -= _messageService.EditHandle;
            ServiceRealizations.EventHandler.OnDelete -= _messageService.DeleteLocal;
            ServiceRealizations.EventHandler.OnEdit -= _messageService.StartEdit;
        }
        await base.DisposeAsync(disposing);
    }
}