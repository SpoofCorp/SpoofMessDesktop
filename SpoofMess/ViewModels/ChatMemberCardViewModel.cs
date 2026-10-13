using SpoofMess.Models;

namespace SpoofMess.ViewModels;

public class ChatMemberCardViewModel : AdditionalViewModel
{
    public ChatUser ChatUser { get; set; } = null!;
}
