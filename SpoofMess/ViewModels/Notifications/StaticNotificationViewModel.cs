using CommunityToolkit.Mvvm.ComponentModel;

namespace SpoofMess.ViewModels.Notifications;

public partial class StaticNotificationViewModel : ObservableObject
{
    [ObservableProperty]
    public partial AdditionalViewModel ViewModel { get; set; }


}
