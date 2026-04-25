using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using CommonObjects.Requests.Messages;
using CommonObjects.Responses;
using Microsoft.AspNetCore.SignalR.Client;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;

namespace SpoofMess.ServiceRealizations.Api;

class NotificationApiService : INotificationApiService, IAsyncDisposable
{
    private readonly IAuthService _authService;
    private readonly HubConnection _connection;

    public event Action<MessageDTO> OnMessageReceived = null!;
    public event Action<EditMessageResponse> OnMessageEdited = null!;
    public event Action<UpdateUserInfo> OnUserUpdated = null!;
    public event Action<ChangeChatSettingsRequest> OnChatUpdated = null!;

    private readonly CancellationTokenSource _cts = new();


    public NotificationApiService(
            IAuthService authService
        )
    {
        _authService = authService;
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
        _connection.On<EditMessageResponse>("edited-message", (message) =>
        {
            OnMessageEdited?.Invoke(message);
        });
        _connection.On<UpdateUserInfo>("user-updated", (userInfo) =>
        {
            OnUserUpdated?.Invoke(userInfo);
        });
        _connection.On("chat-updated", OnChatUpdated);
    }

    public async Task SendMessage(CreateMessageRequest message)
    {
        await _connection.InvokeAsync("SendMessage", message);
    }
    public async Task EditMessage(EditMessageRequest message)
    {
        await _connection.InvokeAsync("EditMessage", message);
    }

    public async ValueTask DisposeAsync()
    {
        await _cts.CancelAsync();
        await _connection.DisposeAsync();
    }

    public async Task<bool> DeleteMessage(MessageModel message)
    {
        try
        {
            await _connection.InvokeAsync("DeleteMessage", message);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
