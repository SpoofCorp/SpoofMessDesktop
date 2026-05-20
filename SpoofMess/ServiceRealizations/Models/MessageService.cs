using CommonObjects.DTO;
using CommonObjects.Requests.Attachments;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.Setters;
using System.Diagnostics;
using System.Windows;

namespace SpoofMess.ServiceRealizations.Models;

public class MessageService(
    IMessageApiService messageApiService,
    IAttachmentService attachmentService,
    IUserService userService,
    INotificationApiService notificationApiService,
    IChatService chatService) : IMessageService
{
    private readonly IUserService _userService = userService;
    private readonly INotificationApiService _notificationApiService = notificationApiService;
    private readonly IMessageApiService _messageApiService = messageApiService;
    private readonly IAttachmentService _attachmentService = attachmentService;
    private readonly IChatService _chatService = chatService;

    public async void UploadMessage(MessageDTO message)
    {
        Chat? chat = await _chatService.Get(message.ChatId);
        if (chat is null)
            return;
        MessageModel messageModel = message.Set();
        AddMessage(messageModel, chat);
        await UploadAdditionalData(messageModel, message, default);
    }
    public async Task LoadSkippedMesssages(DateTime after)
    {
        Result<List<MessageDTO>> chats = await _messageApiService.GetSkippedMessages(after);
        while (chats.Success && chats.Body?.Count > 0)
        {
            if (chats.Success)
            {
                await Parallel.ForEachAsync(chats.Body!, async (messageDTO, cancelationToken) =>
                {
                    MessageModel message = messageDTO.Set();
                    Chat? chat = await _chatService.Get(messageDTO.ChatId);
                    if (chat is null)
                        return;
                    AddMessage(message, chat);
                    await UploadAdditionalData(message, messageDTO, cancelationToken);
                    if (message.SentAt > after)
                        after = message.SentAt;
                });
            }
            if (chats.Body!.Count < 50)
                break;
            chats = await _messageApiService.GetSkippedMessages(after);
        }
    }

    public async void DeleteLocal(MessageModel message)
    {
        if (!await _notificationApiService.DeleteMessage(message))
            return;
        Chat? chat = await _chatService.Get(message.ChatId);
        if (chat is null)
            return;
    }

    public async void StartEdit(MessageModel message)
    {
        Chat? chat = await _chatService.Get(message.ChatId);
        if (chat is null)
            return;
        if (chat.EditedMessage is null)
            chat.EditedMessage = chat.CurrentMessage;
        else
            chat.EditedMessage = new()
            {
                ChatId = chat.Id,
                Text = string.Empty
            };
        chat.CurrentMessage = message.GetEdit();
    }

    public async Task StopEdit(Chat? chat)
    {
        if (chat is null)
            return;
        chat.CurrentMessage = chat.EditedMessage ?? new();
        chat.EditedMessage = null;
    }

    public async Task SendMessage(Chat? chat, CancellationToken token = default)
    {
        if (chat is null) return;
        if (chat.EditedMessage is not null)
        {
            await Edit(chat, token);
            return;
        }
        MessageModel request = chat.CurrentMessage;
        request.ChatId = chat.Id;
        chat.CurrentMessage = new()
        {
            ChatId = chat.Id,
            Text = string.Empty
        };
        Result<List<Attachment>> attachments = await _attachmentService.SendAttachments(request, token);
        if (attachments.Success)
            await _notificationApiService.SendMessage(request.Set(attachments.Body!));
    }

    private async Task Edit(Chat chat, CancellationToken token = default)
    {
        if (chat.EditedMessage is null || chat.CurrentMessage is not EditMessageModel edit)
            return;
        edit.ChatId = chat.Id;
        chat.CurrentMessage = chat.EditedMessage;
        chat.EditedMessage = null;
        Result<List<CommonObjects.Responses.EditAttachment>> attachments = await _attachmentService.SendAttachments(edit, token);
        if (attachments.Success)
            await _notificationApiService.EditMessage(edit.SetEdit(attachments.Body!));
    }

    public async void OnDelete(Guid messageId, Guid chatId)
    {
        Chat? chat = await _chatService.Get(chatId);
        if (chat is null)
            return;
        MessageModel? message = chat.Messages.FirstOrDefault(x => x.Id == messageId);
        if (message is null)
            return;

        Application.Current.Dispatcher.Invoke(() =>
        {
            chat.Messages.Remove(message);
        });
    }

    public async void EditHandle(EditMessageResponse message)
    {
        Chat? chat = await _chatService.Get(message.ChatId);
        if (chat is null)
            return;
        MessageModel? messageModel = chat.Messages.FirstOrDefault(x => x.Id == message.Id);
        if (messageModel is null)
            return;
        messageModel.Text = message.Text is null ? messageModel.Text : message.Text;
        await _attachmentService.UploadAttachments(
            messageModel,
            message.Attachments);
    }

    private static void AddMessage(MessageModel message, Chat chat)
    {
        lock (chat)
        {
            MessageModel? prefixMessage = chat.Messages.FirstOrDefault(x => x.SentAt >= message.SentAt);
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (prefixMessage is null)
                    chat.Messages.Add(message);
                else
                    chat.Messages.Insert(chat.Messages.IndexOf(prefixMessage), message);
            });
            EventHandler.NotifyAdd(message);
        }
    }

    private async Task UploadAdditionalData(MessageModel message, MessageDTO messageDTO, CancellationToken cancelationToken)
    {

        Task userTask = Task.Run(async () =>
        {
            User? user = await _userService.Get(message.User!.Login, cancelationToken);
            if (user is null)
                return;
            user.Name = messageDTO.SenderName;
            user.Login = messageDTO.SenderLogin;

            Application.Current.Dispatcher.Invoke(() =>
            {
                message.User = user;
            });
        }, cancelationToken);
        Task uploadTask = _attachmentService.UploadAttachments(
            message,
            messageDTO.Attachments);
        await Task.WhenAll(userTask, uploadTask);
    }

    public async Task UploadMessagesAfterDate(Chat chat)
    {
        DateTime date = DateTime.UtcNow;
        Result<List<MessageDTO>> chats = await _messageApiService.GetBeforeMessages(chat.Id, date);
        while (chats.Success && chats.Body?.Count > 0)
        {
            if (chats.Success)
            {
                await Parallel.ForEachAsync(chats.Body!, async (messageDTO, cancelationToken) =>
                {
                    MessageModel message = messageDTO.Set();
                    Chat? chat = await _chatService.Get(messageDTO.ChatId);
                    if (chat is null)
                        return;
                    AddMessage(message, chat);
                    await UploadAdditionalData(message, messageDTO, cancelationToken);
                    if (message.SentAt < date)
                        date = message.SentAt;
                });
            }
            if (chats.Body!.Count < 50)
                break;
            chats = await _messageApiService.GetBeforeMessages(chat.Id, date);
        }
    }
}
