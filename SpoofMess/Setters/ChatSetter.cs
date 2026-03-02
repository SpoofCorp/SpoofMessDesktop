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
            UniqueName = chat.UniqueName,
            Messages = []
        };
    public static Chat Set(this ChatUserDTO chat) =>
        new()
        {
            Id = chat.Id,
            Name = chat.Name,
            ChatTypeId = chat.ChatTypeId,
            UniqueName = chat.UniqueName,
            //Rules = [.. chat.Rules]
        };
}
