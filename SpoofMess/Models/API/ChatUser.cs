namespace SpoofMess.Models;

public class ChatUser
{
    public User User { get; set; } = null!;
    public Chat Chat { get; set; } = null!;
}