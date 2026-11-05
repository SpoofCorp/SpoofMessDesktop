using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models.UpdateModels;
using System.Globalization;
using System.Windows;

namespace SpoofMess.Models;

public partial class AppSessionSettings : ObservableObject
{
    [ObservableProperty]
    public partial double Width { get; set; } = SystemParameters.PrimaryScreenWidth / 2;

    [ObservableProperty]
    public partial double Height { get; set; } = SystemParameters.FullPrimaryScreenHeight / 1.75;

    [ObservableProperty]
    public partial bool HideToTray { get; set; }

    [ObservableProperty]
    public partial float Volume { get; set; }
    [ObservableProperty]
    public partial Language Language { get; set; } = new("English", "En", "en-US");
    [ObservableProperty]
    public partial Theme Theme { get; set; } = new("Light");
    [ObservableProperty]
    public partial Visual Visual { get; set; } = new("OldBase");

    [ObservableProperty]
    public partial bool CloneWindows { get; set; }

    public CultureInfo CurrentCultureInfo = new("en-US");

    [ObservableProperty]
    public partial bool UnselectChat { get; set; }

    [ObservableProperty]
    public partial bool ShowMessagesNotifications { get; set; }

    [ObservableProperty]
    public partial bool ShowErrorNotifications { get; set; } = true;
    [ObservableProperty]
    public partial AudioInDevice? AudioInDevice { get; set; }


    public bool Update(UserInfoUpdateModel userInfoUpdateModel)
    {
        int hash = GetHashCode();
        CloneWindows = userInfoUpdateModel.CloneWindows ?? CloneWindows;
        Height = userInfoUpdateModel.Height ?? Height;
        Width = userInfoUpdateModel.Width ?? Width;
        HideToTray = userInfoUpdateModel.HideToTray ?? HideToTray;
        UnselectChat = userInfoUpdateModel.UnselectChat ?? UnselectChat;
        Language = userInfoUpdateModel.Language ?? Language;
        Theme = userInfoUpdateModel.Theme ?? Theme;
        Visual = userInfoUpdateModel.Visual ?? Visual;
        AudioInDevice = userInfoUpdateModel.AudioInDevice ?? AudioInDevice;
        ShowMessagesNotifications = userInfoUpdateModel.ShowMessagesNotifications ?? ShowMessagesNotifications;
        ShowErrorNotifications = userInfoUpdateModel.ShowErrorNotifications ?? ShowErrorNotifications;
        return hash != GetHashCode();
    }
    public void Init(AppSessionSettings appSessionSettings)
    {
        Volume = appSessionSettings.Volume;
        CloneWindows = appSessionSettings.CloneWindows;
        Height = appSessionSettings.Height;
        Width = appSessionSettings.Width;
        HideToTray = appSessionSettings.HideToTray;
        UnselectChat = appSessionSettings.UnselectChat;
        Language = appSessionSettings.Language;
        Theme = appSessionSettings.Theme;
        AudioInDevice = appSessionSettings.AudioInDevice;
        Visual = appSessionSettings.Visual;
        ShowMessagesNotifications = appSessionSettings.ShowMessagesNotifications;
        ShowErrorNotifications = appSessionSettings.ShowErrorNotifications;
    }
}
