using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Enums;

namespace SpoofMess.Models.API;

public partial class Notification(string? text, NotificationType type) : ObservableObject
{
    public string? Text
    {
        get => field ?? "Undefined";
        set => field = value;
    } = text;

    public NotificationType Type { get; set; } = type;

    [ObservableProperty]
    private double _opacity = 1;

    public int Time => Type switch { Enums.NotificationType.Error => 4000, Enums.NotificationType.Info => 2000, _ => 3000 };
}
