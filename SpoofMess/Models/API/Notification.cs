using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Enums;

namespace SpoofMess.Models.API;

public partial class Notification(string? text, NotificationType type) : ObservableObject
{
    public string? Text
    {
        get => field ?? "Undefined";
        set;
    } = text;

    public NotificationType Type { get; set; } = type;

    [ObservableProperty]
    public partial double Opacity { get; set; } = 1;

    public TimeSpan Time => Type switch { NotificationType.Error => TimeSpan.FromMilliseconds(4000), NotificationType.Info => TimeSpan.FromMilliseconds(2000), _ => TimeSpan.FromMilliseconds(3000) };
}
