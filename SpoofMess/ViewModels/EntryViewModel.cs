using CommunityToolkit.Mvvm.ComponentModel;

namespace SpoofMess.ViewModels;

internal partial class EntryViewModel(
    ) : ObservableObject
{
    [ObservableProperty]
    public partial object ViewModel { get; set; } = null!;
}
