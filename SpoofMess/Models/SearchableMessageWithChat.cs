namespace SpoofMess.Models;

public record SearchableMessageWithChat(
    Guid Id,
    string? Text,
    DateTime SentAt,
    Chat Chat);