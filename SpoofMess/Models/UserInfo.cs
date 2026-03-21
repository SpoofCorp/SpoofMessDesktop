using CommonObjects.Responses;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Text.Json.Serialization;

namespace SpoofMess.Models;

public partial class UserInfo : ObservableObject
{
    [ObservableProperty]
    private string _name = string.Empty;
    [ObservableProperty]
    private string _login = string.Empty;
    [JsonIgnore]
    [ObservableProperty]
    private string _password = string.Empty;
    public UserAuthorizeResponse? Authorize { get; set; }
    public SessionSettings SessionSettings { get; set; } = new();

    public void Update(UserInfo userInfo)
    {
        Name = userInfo.Name;
        Login = userInfo.Login;
        Authorize = userInfo.Authorize;
        SessionSettings = userInfo.SessionSettings;
    }
}
