using CommonObjects.Results;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;

namespace SpoofMess.ViewModels;

public partial class ChatInfoViewModel(IChatAvatarService chatAvatarService, INotificationService notificationService) : AdditionalViewModel
{
    private readonly IChatAvatarService _chatAvatarService = chatAvatarService;
    private readonly INotificationService _notificationService = notificationService;
    public Chat Chat { get; set; } = null!;

    [RelayCommand]
    private async Task SetAvatar()
    {
        Result result = await _chatAvatarService.Set(Chat);
        if (!result.Success)
            _notificationService.ShowError(result.Error);
    }
}
