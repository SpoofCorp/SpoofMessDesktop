using CommonObjects.DTO;
using SpoofMess.Models;

namespace SpoofMess.Setters;

public static class ChatSetter
{
    public static Chat Set(this ChatDTO chat) =>
        new()
        {
            Id = chat.Id,
            Name = chat.Name,
            UniqueName = chat.UniqueName
        };
    public static Chat Set(this ChatUserDTO chat) =>
        new()
        {
            Id = chat.ChatId,
            Name = chat.Name,
            UniqueName = chat.UniqueName,
            CreatedAt = chat.CreatedAt,
            RulesNumbers = chat.Rules
        };
}
