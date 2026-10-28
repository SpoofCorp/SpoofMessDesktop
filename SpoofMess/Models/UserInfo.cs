using CommonObjects.Requests.Changes;
using CommonObjects.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models.UpdateModels;
using System.Globalization;
using System.Text.Json.Serialization;
using System.Windows;

namespace SpoofMess.Models;

public partial class UserInfo : ObservableObject
{
    [JsonIgnore]
    [ObservableProperty]
    public partial string Password { get; set; } = string.Empty;
    [ObservableProperty]
    public partial double Width { get; set; } = SystemParameters.PrimaryScreenWidth / 2;

    [ObservableProperty]
    public partial double Height { get; set; } = SystemParameters.FullPrimaryScreenHeight / 1.75;

    [ObservableProperty]
    public partial bool HideToTray { get; set; }

    [ObservableProperty]
    public partial bool UnselectChat { get; set; }

    [ObservableProperty]
    public partial bool ShowMessagesNotifications { get; set; }

    [ObservableProperty]
    public partial bool ShowErrorNotifications { get; set; } = true;

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
    [ObservableProperty]
    public partial Language Language { get; set; } = new("English", "En", "en-US");
    [ObservableProperty]
    public partial Theme Theme { get; set; } = new("Light");
    [ObservableProperty]
    public partial Visual Visual { get; set; } = new("OldBase");

    [ObservableProperty]
    public partial bool CloneWindows { get; set; }

    public CultureInfo CurrentCultureInfo = new("en-US");

    public UserAuthorizeResponse? Authorize { get; set; }

    public SessionSettings SessionSettings { get; set; } = new();

    public User User { get; set; } = new();

    public bool Update(UserInfoUpdateModel userInfoUpdateModel)
    {
        int hash = GetHashCode();
        Authorize = userInfoUpdateModel.Authorize ?? Authorize;
        SessionSettings = userInfoUpdateModel.SessionSettings ?? SessionSettings;
        CloneWindows = userInfoUpdateModel.CloneWindows ?? CloneWindows;
        Height = userInfoUpdateModel.Height ?? Height;
        Width = userInfoUpdateModel.Width ?? Width;
        HideToTray = userInfoUpdateModel.HideToTray ?? HideToTray;
        UnselectChat = userInfoUpdateModel.UnselectChat ?? UnselectChat;
        SearchMe = userInfoUpdateModel.SearchMe ?? SearchMe;
        ShowMe = userInfoUpdateModel.ShowMe ?? ShowMe;
        ForwardMessage = userInfoUpdateModel.ForwardMessage ?? ForwardMessage;
        InviteMe = userInfoUpdateModel.InviteMe ?? InviteMe;
        MonthsBeforeDelete = userInfoUpdateModel.MonthsBeforeDelete ?? MonthsBeforeDelete;
        Language = userInfoUpdateModel.Language ?? Language;
        Theme = userInfoUpdateModel.Theme ?? Theme;
        Visual = userInfoUpdateModel.Visual ?? Visual;
        ShowMessagesNotifications = userInfoUpdateModel.ShowMessagesNotifications ?? ShowMessagesNotifications;
        ShowErrorNotifications = userInfoUpdateModel.ShowErrorNotifications ?? ShowErrorNotifications;
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
        SessionSettings = userInfo.SessionSettings;
        CloneWindows = userInfo.CloneWindows;
        Height = userInfo.Height;
        Width = userInfo.Width;
        HideToTray = userInfo.HideToTray;
        UnselectChat = userInfo.UnselectChat;
        SearchMe = userInfo.SearchMe;
        ShowMe = userInfo.ShowMe;
        ForwardMessage = userInfo.ForwardMessage;
        InviteMe = userInfo.InviteMe;
        MonthsBeforeDelete = userInfo.MonthsBeforeDelete;
        Language = userInfo.Language;
        Theme = userInfo.Theme;
        Visual = userInfo.Visual;
        ShowMessagesNotifications = userInfo.ShowMessagesNotifications;
        ShowErrorNotifications = userInfo.ShowErrorNotifications;
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

        if (edit.ShowErrorNotifications != ShowErrorNotifications)
        {
            result = true;
        }

        if (edit.ShowMessagesNotifications != ShowMessagesNotifications)
        {
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
