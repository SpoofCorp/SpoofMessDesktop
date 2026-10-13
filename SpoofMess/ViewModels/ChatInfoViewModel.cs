using CommonObjects.Results;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;

namespace SpoofMess.ViewModels;

public partial class ChatInfoViewModel(
    IChatAvatarService chatAvatarService,
    INotificationService notificationService,
    INavigationService navigationService,
    IChatUserService chatUserService) : PlacementAdditionalViewModel, IAsyncInitializable
{
    private readonly IChatAvatarService _chatAvatarService = chatAvatarService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly IChatUserService _chatUserService = chatUserService;

    public Chat Chat { get; set; } = null!;

    public bool IsInitialized { get; set; }


    [RelayCommand]
    private async Task SetAvatar()
    {
        Result result = await _chatAvatarService.Set(Chat);
        if (!result.Success)
            _notificationService.ShowError(result.Error);
    }

    [RelayCommand]
    private async Task ScrollToBottom()
    {
        await _chatUserService.UploadManyMembers(Chat);
    }
    [RelayCommand]
    private async Task Mute()
    {

    }
    [RelayCommand]
    private async Task Ban()
    {

    }
    [RelayCommand]
    private async Task Kick()
    {

    }

    public async Task InitializeAsync()
    {
        if (Chat.Users.Count == 0)
            await _chatUserService.UploadManyMembers(Chat);
        IsInitialized = true;
    }

    [RelayCommand]
    private void ShowMemberCard(ChatUser chatUser)
    {
        _navigationService.SetChatMemberCard(this, chatUser);
    }
}
