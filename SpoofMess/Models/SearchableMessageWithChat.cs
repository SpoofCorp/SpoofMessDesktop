namespace SpoofMess.Models;

public record class SearchableMessageWithChat(
    string? Text,
    DateTime SentAt,
    Chat Chat
) : ISearchableEntity
{
    public Guid Id { get; set; }
}