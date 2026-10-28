using CommonObjects.Requests.Changes;
using SpoofMess.Models;
using SpoofMess.Models.UpdateModels;
using SpoofMess.ServiceRealizations;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class SoundsAndNotificationsViewModel(UserInfo userInfo,
                                                     IAuthService authService,
                                                     IAppLifeCycleService appLifeCycleService) : AdditionalViewModel(appLifeCycleService)
{
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
        if (_actualUserInfo.Change(Edit, out ChangeUserSettingsRequest request))
        {
            await _authService.Save(_appLifeCycleService.AppCancellationToken);
            _actualUserInfo.Update(new UserInfoUpdateModel(
                    ForwardMessage: request.ForwardMessage,
                    InviteMe: request.InviteMe,
                    MonthsBeforeDelete: request.MonthsBeforeDelete,
                    UserUpdateModel: new(Name: request.Name),
                    SearchMe: request.SearchMe,
                    ShowMe: request.ShowMe
            ));
        }
        base.OnClose();
    }
}
