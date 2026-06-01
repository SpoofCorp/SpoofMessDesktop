using CommonObjects.Requests.Changes;
using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class SoundsAndNotificationsViewModel : AdditionalViewModel
{
    private readonly IAuthService _authService;
    private readonly UserInfo _actualUserInfo;
    public UserInfo Edit { get; set; }

    public SoundsAndNotificationsViewModel(UserInfo userInfo, IAuthService authService)
    {
        _authService = authService;
        _actualUserInfo = userInfo;
        Edit = new();
        Edit.Update(userInfo);
    }

    public override async void OnClose()
    {
        if (_actualUserInfo.Change(Edit, out ChangeUserSettingsRequest request))
        {
            await _authService.Save();
            _actualUserInfo.Update(Edit);
        }
        base.OnClose();
    }
}
