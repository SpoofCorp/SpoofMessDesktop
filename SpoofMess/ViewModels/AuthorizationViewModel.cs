using CommonObjects.Responses;
using CommonObjects.Results;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;

namespace SpoofMess.ViewModels;

public partial class AuthorizationViewModel(
     IEntryApiService entryApiService,
     INavigationService navigationService,
     INotificationService notificationService,
     UserInfo userInfo,
     IAppLifeCycleService appLifeCycleService) : BaseViewModel(appLifeCycleService)
{
    private readonly IEntryApiService _entryApiService = entryApiService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly INotificationService _notificationService = notificationService;

    public UserInfo UserInfo { get; set; } = userInfo;

    [RelayCommand]
    private void ChangeView() =>
        _navigationService.ShowRegistrationView();
    
    [RelayCommand]
    private async Task Enter()
    {
        Result<UserAuthorizeResponse> result = await _entryApiService.Enter(
                new()
                {
                    Login = UserInfo.User.Login,
                    Password = UserInfo.Password
                },
                _appLifeCycleService.AppCancellationToken
            );
        if (result)
            _navigationService.ShowMainView();
        else
            _notificationService.ShowError(result);
    }
}
