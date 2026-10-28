using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels;

public class ChatMemberCardViewModel(IAppLifeCycleService appLifeCycleService) : AdditionalViewModel(appLifeCycleService)
{
    public ChatUser ChatUser { get; set; } = null!;
}
