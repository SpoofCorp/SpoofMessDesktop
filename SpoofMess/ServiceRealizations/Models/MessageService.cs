using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.Setters;

namespace SpoofMess.ServiceRealizations.Models;

public class MessageService(
    IMessageApiService messageApiService,
    IAttachmentService attachmentService,
    IUserService userService,
    IChatService chatService) : IMessageService
{
    private readonly IUserService _userService = userService;
    private readonly IMessageApiService _messageApiService = messageApiService;
    private readonly IAttachmentService _attachmentService = attachmentService;
    private readonly IChatService _chatService = chatService;

    public async Task UploadMessage(MessageDTO message)
    {
        Chat? chat = await _chatService.Get(message.ChatId);
        if (chat is null)
            return;
        //Need to check sender
        MessageModel messageModel = message.Set();
        AddMessage(messageModel, chat);
       await _attachmentService.UploadAttachments(messageModel, message.Attachments);
    }

    public async Task LoadSkippedMesssages(DateTime after)
    {
        Result<List<MessageDTO>> chats = await _messageApiService.GetSkippedMessages(after);
        while (chats.Success && chats.Body?.Count > 0)
        {
            chats = await _messageApiService.GetSkippedMessages(after);
            if (chats.Success)
            {
                await Parallel.ForEachAsync(chats.Body!, async (messageDTO, cancelationToken) =>
                {
                    MessageModel message = messageDTO.Set();
                    Chat? chat = await _chatService.Get(messageDTO.ChatId);
                    if (chat is null)
                        return;
                    AddMessage(message, chat);
                    Task userTask = Task.Run(async () =>
                    {
                        User? user = await _userService.Get(message.User!.Login);
                        if (user is null)
                            return;

                        App.Current.Dispatcher.Invoke(() =>
                        {
                            message.User = user;
                        });
                    }, cancelationToken);
                    Task uploadTask = _attachmentService.UploadAttachments(
                        message,
                        messageDTO.Attachments);
                    await Task.WhenAll(userTask, uploadTask);
                    if(message.SentAt > after)
                        after = message.SentAt;
                });
                GC.Collect();
            }
            if (chats.Body!.Count < 50)
                break;
        }
    }

    private static void AddMessage(MessageModel message, Chat chat)
    {
        lock (chat)
        {
            MessageModel? prefixMessage = chat.Messages.FirstOrDefault(x => x.SentAt >= message.SentAt);
            App.Current.Dispatcher.Invoke(() =>
            {
                if (prefixMessage is null)
                    chat.Messages.Add(message);
                else
                    chat.Messages.Insert(chat.Messages.IndexOf(prefixMessage), message);
            });
        }
    }
}
