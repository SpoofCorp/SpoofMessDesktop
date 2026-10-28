using CommonObjects.DTO;
using CommonObjects.Requests.Changes;
using CommonObjects.Requests.Messages;
using CommonObjects.Responses;
using Microsoft.AspNetCore.SignalR.Client;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using System.Diagnostics;

namespace SpoofMess.ServiceRealizations.Api;

class NotificationApiService : INotificationApiService, IAsyncDisposable
{
    private readonly IAuthService _authService;
    private readonly HubConnection _connection;

    public event Action<MessageDTO> OnMessageReceived = null!;
    public event Action<Guid, Guid> OnMessageDeleted = null!;
    public event Action<EditMessageResponse> OnMessageEdited = null!;
    public event Action<UpdateUserInfo> OnUserUpdated = null!;
    public event Action<ChangeChatSettingsRequest> OnChatUpdated = null!;
    public event Action<ChatAvatarResponse> OnChatAvatarUpdated = null!;
    public event Action<ChatUserDTO> OnChatCreated = null!;
    private readonly SemaphoreSlim _initializeSemaphore = new(1, 1);

    private readonly CancellationTokenSource _cts = new();

    public bool IsInitialized { get; set; }

    public NotificationApiService(
            IAuthService authService
        )
    {
        _authService = authService;
        _connection = new HubConnectionBuilder()
            .WithUrl($"https://localhost:443/sms/chat", options =>
            {
                options.AccessTokenProvider = () => _authService.GetAccess();

            })
            .WithAutomaticReconnect()
            .Build();
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
            return;
        await _initializeSemaphore.WaitAsync(cancellationToken);
        try
        {
            if (IsInitialized)
                return;

            _connection.On("chat-user-created", OnChatCreated);
            _connection.On("new-message", OnMessageReceived);
            _connection.On("delete-message", OnMessageDeleted);
            _connection.On("edited-message", OnMessageEdited);
            _connection.On("user-updated", OnUserUpdated);
            _connection.On("chat-updated", OnChatUpdated);
            _connection.On("chat-avatar-updated", OnChatAvatarUpdated);

            await _connection.StartAsync(cancellationToken);
        }
        catch(Exception ex)
        {
            Debug.WriteLine(ex.Message);
        }
        finally
        {
            IsInitialized = true;
            _initializeSemaphore.Release();
        }
    }

    public async Task SendMessage(CreateMessageRequest message, CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync("SendMessage", message, cancellationToken);
    }
    public async Task EditMessage(EditMessageRequest message, CancellationToken cancellationToken = default)
    {
        await _connection.InvokeAsync("EditMessage", message, cancellationToken);
    }

    public async ValueTask DisposeAsync()
    {
        _initializeSemaphore.Dispose();
        await _cts.CancelAsync();
        await _connection.DisposeAsync();
        GC.SuppressFinalize(this);
    }

    public async Task<bool> DeleteMessage(MessageModel message, CancellationToken cancellationToken = default)
    {
        try
        {
            await _connection.InvokeAsync("DeleteMessage", message.ChatId, message.Id, cancellationToken);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
