using CommonObjects.DTO;
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
            UserId = message.UserId,
            User = new()
            {
                Id = message.UserId,
                Name = message.UserName
            }
        };
}
