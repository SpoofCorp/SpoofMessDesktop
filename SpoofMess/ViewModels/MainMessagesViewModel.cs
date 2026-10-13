using SpoofMess.ServiceRealizations;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;

namespace SpoofMess.ViewModels;

public partial class MainMessagesViewModel(
    IMessageService messageService,
    INotificationService notificationService,
    IAttachmentService attachmentService,
    FileAudioRecorderService fileAudioRecorderService,
    INotificationApiService notificationApiService) : MessagesViewModel(messageService, notificationService, attachmentService, fileAudioRecorderService)
{
    private readonly INotificationApiService _notificationApiService = notificationApiService;

    public override async Task InitializeAsync()
    {
        _notificationApiService.OnMessageReceived += _messageService.UploadMessage;
        _notificationApiService.OnMessageEdited += _messageService.EditHandle;
        _notificationApiService.OnMessageDeleted += _messageService.OnDelete;
        ServiceRealizations.EventHandler.OnDelete += _messageService.DeleteLocal;
        ServiceRealizations.EventHandler.OnEdit += _messageService.StartEdit;
        await _messageService.LoadSkippedMesssages(DateTime.UtcNow.AddMonths(-10));
        await base.InitializeAsync();
    }

    public override void Dispose()
    {
        _notificationApiService.OnMessageReceived -= _messageService.UploadMessage;
        _notificationApiService.OnMessageDeleted -= _messageService.OnDelete;
        _notificationApiService.OnMessageEdited -= _messageService.EditHandle;
        ServiceRealizations.EventHandler.OnDelete -= _messageService.DeleteLocal;
        ServiceRealizations.EventHandler.OnEdit -= _messageService.StartEdit;
        GC.SuppressFinalize(this);
        base.Dispose();
    }
}