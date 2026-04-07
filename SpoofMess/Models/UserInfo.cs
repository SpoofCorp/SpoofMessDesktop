using CommonObjects.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;
using System.Windows;

namespace SpoofMess.Models;

public partial class UserInfo : ObservableObject
{
    [JsonIgnore]
    [ObservableProperty]
    private string _password = string.Empty;

    public UserAuthorizeResponse? Authorize { get; set; }

    public SessionSettings SessionSettings { get; set; } = new();

    public User User { get; set; } = new();

    [ObservableProperty]
    private double _width = SystemParameters.PrimaryScreenWidth / 2;
    [ObservableProperty]
    private double _height = SystemParameters.FullPrimaryScreenHeight / 1.75;

    public void Update(UserInfo userInfo)
    {
        User = userInfo.User;
        Authorize = userInfo.Authorize;
        SessionSettings = userInfo.SessionSettings;
        Height = userInfo.Height;
        Width = userInfo.Width;
    }
}
