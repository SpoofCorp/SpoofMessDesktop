using CommonObjects.DTO;
using CommonObjects.Requests.Messages;
using Microsoft.AspNetCore.SignalR.Client;
using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ServiceRealizations;

class MessageService : IMessageService, IAsyncDisposable
{
    private readonly IAuthService _authService;
    private readonly HubConnection _connection;

    public event Action<MessageModel> OnMessageReceived = null!;


    public MessageService(
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
        _connection.StartAsync().Wait();
        _connection.On<MessageDTO>("new-message", OnMessage);
    }

    private readonly CancellationTokenSource _cts = new();

    private async Task OnMessage(MessageDTO message)
    {
        //Need to extract to setter or constructor
        MessageModel messageModel = new()
        {
            ChatId = message.ChatId,
            SentAt = DateTime.UtcNow,
            Text = message.Text,
            UserId = message.UserId,
            User = new()
            {
                Name = message.UserName,
                Id = message.UserId,
                AvatarId = message.UserAvatarId
            }
        };
        OnMessageReceived.Invoke(messageModel);
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
