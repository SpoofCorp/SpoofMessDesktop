using CommonObjects.DTO;
using CommonObjects.Requests.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using SpoofMess.Models;
using SpoofMess.ServiceRealizations.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;

namespace SpoofMess.ServiceRealizations.Api;

class NotificationApiService : INotificationApiService, IAsyncDisposable
{
    private readonly IAttachmentService _attachmentService;
    private readonly IFileService _fileService;
    private readonly IAuthService _authService;
    private readonly HubConnection _connection;

    public event Action<MessageDTO> OnMessageReceived = null!;

    private readonly CancellationTokenSource _cts = new();


    public NotificationApiService(
            IAttachmentService attachmentService,
            IAuthService authService,
            IFileService fileService
        )
    {
        _attachmentService = attachmentService;
        _authService = authService;
        _fileService = fileService;
        _connection = new HubConnectionBuilder()
            .WithUrl("https://localhost:7146/chat", options =>
            {
                options.AccessTokenProvider = () => _authService.GetAccess();
            })
            .WithAutomaticReconnect()
            .Build();
        //It's so bad
        InitializeAsync();
    }

    private async void InitializeAsync()
    {
        await _connection.StartAsync();
        _connection.On<MessageDTO>("new-message", (message) =>
        {
            OnMessageReceived?.Invoke(message);
        });
    }

    public async Task SendMessage(CreateMessageRequest message)
    {
        await _connection.InvokeAsync("SendMessage", message);
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        await _connection.DisposeAsync();
    }

}
