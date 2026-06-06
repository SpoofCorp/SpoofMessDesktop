using CommonObjects.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;

namespace SpoofMess.ViewModels;

public partial class ChatInfoViewModel(IChatAvatarService chatAvatarService, INotificationService notificationService, IChatUserService chatUserService) : AdditionalViewModel, IAsyncInitializable
{
    private readonly IChatAvatarService _chatAvatarService = chatAvatarService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IChatUserService _chatUserService = chatUserService;

    public Chat Chat { get; set; } = null!;

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

    public override async void Initialize(ObservableObject ownerView, Action close)
    {
        base.Initialize(ownerView, close);
    }

    public async Task InitiailizeAsync()
    {
        if (Chat.Users.Count == 0)
            await _chatUserService.UploadManyMembers(Chat);
    }
}
