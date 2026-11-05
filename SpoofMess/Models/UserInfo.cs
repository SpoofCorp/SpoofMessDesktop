using CommonObjects.Requests.Changes;
using CommonObjects.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models.UpdateModels;
using System.Text.Json.Serialization;

namespace SpoofMess.Models;

public partial class UserInfo : ObservableObject
{
    [JsonIgnore]
    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;

    [ObservableProperty]
    public partial bool SearchMe { get; set; }

    [ObservableProperty]
    public partial bool ShowMe { get; set; }

    [ObservableProperty]
    public partial bool ForwardMessage { get; set; }

    [ObservableProperty]
    public partial bool InviteMe { get; set; }

    [ObservableProperty]
    public partial int MonthsBeforeDelete { get; set; }

    [ObservableProperty]
    public partial string EditedName { get; set; } = string.Empty;

    public UserAuthorizeResponse? Authorize { get; set; }

    public User User { get; set; } = new();

    public bool Update(UserInfoUpdateModel userInfoUpdateModel)
    {
        int hash = GetHashCode();
        Authorize = userInfoUpdateModel.Authorize ?? Authorize;
        SearchMe = userInfoUpdateModel.SearchMe ?? SearchMe;
        ShowMe = userInfoUpdateModel.ShowMe ?? ShowMe;
        ForwardMessage = userInfoUpdateModel.ForwardMessage ?? ForwardMessage;
        InviteMe = userInfoUpdateModel.InviteMe ?? InviteMe;
        MonthsBeforeDelete = userInfoUpdateModel.MonthsBeforeDelete ?? MonthsBeforeDelete;
        EditedName = userInfoUpdateModel.EditedName ?? User.Name ?? string.Empty;
        if (userInfoUpdateModel.UserUpdateModel is not null)
            User.Update(userInfoUpdateModel.UserUpdateModel);
        return hash != GetHashCode();
    }

    public void Init(UserInfo userInfo)
    {
        EditedName = userInfo.User.Name ?? string.Empty;
        User.Init(userInfo.User);
        Authorize = userInfo.Authorize;
        SearchMe = userInfo.SearchMe;
        ShowMe = userInfo.ShowMe;
        ForwardMessage = userInfo.ForwardMessage;
        InviteMe = userInfo.InviteMe;
        MonthsBeforeDelete = userInfo.MonthsBeforeDelete;
    }

    public bool Change(UserInfo edit, out ChangeUserSettingsRequest request)
    {
        bool result = false;
        request = new();
        if (edit.ForwardMessage != ForwardMessage)
        {
            request.ForwardMessage = edit.ForwardMessage;
            result = true;
        }
        if (edit.InviteMe != InviteMe)
        {
            request.InviteMe = edit.InviteMe;
            result = true;
        }

        if (edit.SearchMe != SearchMe)
        {
            request.SearchMe = edit.SearchMe;
            result = true;
        }

        if (edit.ShowMe != ShowMe)
        {
            request.ShowMe = edit.ShowMe;
            result = true;
        }

        if (edit.MonthsBeforeDelete != MonthsBeforeDelete)
        {
            request.MonthsBeforeDelete = edit.MonthsBeforeDelete;
            result = true;
        }

        if (edit.EditedName != User.Name)
        {
            edit.User.Name = edit.EditedName;
            request.Name = edit.EditedName;
            result = true;
        }
        return result;
    }
}
