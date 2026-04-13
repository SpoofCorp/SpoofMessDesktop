using CommonObjects.Requests.Changes;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services.Api;

namespace SpoofMess.ViewModels.Settings;

public partial class ProfileViewModel : AdditionalViewModel
{
    private readonly IUserApiService _userApiService;
    private readonly UserInfo _actualUserInfo;
    public UserInfo Edit { get; set; }
    public ProfileViewModel(UserInfo userInfo, IUserApiService userApiService)
    {
        _actualUserInfo = userInfo;
        Edit = new();
        Edit.Update(userInfo);
        _userApiService = userApiService;
    }

    public override async void OnClose()
    {
        if(_actualUserInfo.Change(Edit, out ChangeUserSettingsRequest request))
        {
            Result result = await _userApiService.ChangeSettings(request);
            if (result.Success)
                _actualUserInfo.Update(Edit);
        }
        base.OnClose();
    }
}
