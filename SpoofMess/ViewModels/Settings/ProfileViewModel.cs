using CommonObjects.Requests.Changes;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;

namespace SpoofMess.ViewModels.Settings;

public partial class ProfileViewModel(UserInfo userInfo,
                                      IUserApiService userApiService,
                                      IAuthService authService,
                                      IAppLifeCycleService appLifeCycleService) : AdditionalViewModel(appLifeCycleService)
{
    private readonly IUserApiService _userApiService = userApiService;
    private readonly IAuthService _authService = authService;
    private readonly UserInfo _actualUserInfo = userInfo;
    public UserInfo Edit { get; set; } = null!;

    protected override Task InitializeAsync(bool initialize)
    {
        Edit = new();
        Edit.Init(_actualUserInfo);
        return base.InitializeAsync(initialize);
    }

    public override async void OnClose()
    {
        if(_actualUserInfo.Change(Edit, out ChangeUserSettingsRequest request))
        {
            Result result = await _userApiService.ChangeSettings(request, _appLifeCycleService.AppCancellationToken);
            if (result.Success)
            {
                _actualUserInfo.Update(new(
                    ForwardMessage: request.ForwardMessage,
                    InviteMe: request.InviteMe,
                    MonthsBeforeDelete: request.MonthsBeforeDelete,
                    UserUpdateModel: new(Name: request.Name),
                    SearchMe: request.SearchMe,
                    ShowMe: request.ShowMe
                ));
                await _authService.Save(_appLifeCycleService.AppCancellationToken);
            }
        }
        base.OnClose();
    }
}
