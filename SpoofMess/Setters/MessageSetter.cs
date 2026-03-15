using CommonObjects.DTO;
using CommonObjects.Requests.Attachments;
using CommonObjects.Requests.Messages;
using SpoofMess.Models;

namespace SpoofMess.Setters;

public static class MessageSetter
{
    public static MessageModel Set(this MessageDTO message) =>
        new()
        {
            ChatId = message.ChatId,
            SentAt = message.SendAt,
            Text = message.Text,
            User = new()
            {
                Login = message.SenderLogin,
                Name = message.SenderName,
                AvatarId = message.UserAvatarId
            },/*
            Attachments = [..message.Attachments.Select(x => new FileObject() {
                Name = x.OriginalFileName,
                Size = x.Size,
                Token = x.Token
            })]*/
        };
    public static CreateMessageRequest Set(this MessageModel message, List<Attachment> attachments) =>
        new()
        {
            ChatId = message.ChatId,
            Text = message.Text ?? "",
            Attachments = attachments,
        };
}
