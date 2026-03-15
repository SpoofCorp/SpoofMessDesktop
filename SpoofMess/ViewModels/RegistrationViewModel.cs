using CommonObjects.Responses;
using CommonObjects.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;

namespace SpoofMess.ViewModels;

public partial class RegistrationViewModel(
     IEntryApiService entryApiService,
     INavigationService navigationService,
     INotificationService notificationService,
     UserInfo userInfo
    ) : ObservableObject
{
    private readonly IEntryApiService _entryApiService = entryApiService;
    private readonly INavigationService _navigationService = navigationService;
    private readonly INotificationService _notificationService = notificationService;

    public UserInfo UserInfo { get; set; } = userInfo;

    [RelayCommand]
    private void ChangeView()
    {
        _navigationService.GetAuthorizationViewModel();
    }

    [RelayCommand]
    private async Task Registration()
    {
        Result<UserAuthorizeResponse> result = await _entryApiService.Registration(
                new()
                {
                    Login = UserInfo.Login,
                    Password = UserInfo.Password,
                    Name = UserInfo.Name
                }
            );
        if (result.Success)
            _navigationService.ShowMainView();
        else
            _notificationService.ShowToast(new()
            {
                Text = result.Error,
                Type = Enums.NotificationType.Fail
            });
    }
}