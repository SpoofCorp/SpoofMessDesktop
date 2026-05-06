using CommonObjects.Requests.Changes;
using CommonObjects.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Enums;
using System.Text.Json.Serialization;
using System.Windows;

namespace SpoofMess.Models;

public partial class UserInfo : ObservableObject
{
    [JsonIgnore]
    [ObservableProperty]
    private string _password = string.Empty;

    [ObservableProperty]
    private double _width = SystemParameters.PrimaryScreenWidth / 2;

    [ObservableProperty]
    private double _height = SystemParameters.FullPrimaryScreenHeight / 1.75;

    [ObservableProperty]
    private bool _hideToTray;

    [ObservableProperty]
    private bool _unselectChat;

    [ObservableProperty]
    private bool _searchMe;

    [ObservableProperty]
    private bool _showMe;

    [ObservableProperty]
    private bool _forwardMessage;

    [ObservableProperty]
    private bool _inviteMe;

    [ObservableProperty]
    private int _monthsBeforeDelete;

    [ObservableProperty]
    private string _editedName = string.Empty;
    [ObservableProperty]
    private Language _language = Language.Ru;
    [ObservableProperty]
    private Theme _theme = Theme.Light;

    public UserAuthorizeResponse? Authorize { get; set; }

    public SessionSettings SessionSettings { get; set; } = new();

    public User User { get; set; } = new();

    public void Update(UserInfo userInfo)
    {
        EditedName = userInfo.User.Name ?? string.Empty;
        User.Update(userInfo.User);
        Authorize = userInfo.Authorize;
        SessionSettings = userInfo.SessionSettings;
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
