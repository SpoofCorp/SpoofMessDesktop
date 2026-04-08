using CommonObjects.DTO;
using CommonObjects.Requests.Attachments;
using CommonObjects.Requests.Messages;
using CommonObjects.Responses;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.Setters;
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
        Application.Current.Dispatcher.Invoke(() =>
        {
            chat.Messages.Remove(message);
        });
    }

    public async void StartEdit(MessageModel message)
    {
        Chat? chat = await _chatService.Get(message.ChatId);
        if (chat is null)
            return;

        chat._editedMessage = chat.CurrentMessage;
        chat.CurrentMessage = message.GetEdit();
    }

    public async Task StopEdit(MessageModel message, Chat? chat)
    {
        if (chat is not null && chat.Id == message.ChatId)
        {
            chat.CurrentMessage = chat._editedMessage ?? new();
            chat._editedMessage = null;
        }
        else
        {
            Chat? currentChat = await _chatService.Get(message.ChatId);
            if (currentChat is null)
                return;

            currentChat.CurrentMessage = currentChat._editedMessage ?? new();
            currentChat._editedMessage = null;
        }
    }

    public async Task SendMessage(Chat? chat, CancellationToken token = default)
    {
        if (chat is null) return;
        if(chat._editedMessage is not null)
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
        if (chat._editedMessage is null || chat.CurrentMessage is not EditMessageModel edit)
            return;
        edit.ChatId = chat.Id;
        chat.CurrentMessage = chat._editedMessage;

        Result<List<Attachment>> attachments = await _attachmentService.SendAttachments(edit, token);
        if (attachments.Success)
            await _notificationApiService.EditMessage(edit.SetEdit(attachments.Body!));
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
        }
    }

    private async Task UploadAdditionalData(MessageModel message, MessageDTO messageDTO, CancellationToken cancelationToken)
    {

        Task userTask = Task.Run(async () =>
        {
            User? user = await _userService.Get(
                message.User!.Login,
                message.User.Avatar.Id,
                message.User.Avatar.Token,
                messageDTO.OriginalAvatarName);
            if (user is null)
                return;

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
}
