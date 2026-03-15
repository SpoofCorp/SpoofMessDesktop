using CommunityToolkit.Mvvm.ComponentModel;

namespace SpoofMess.ViewModels;

internal partial class EntryViewModel(
    ) : ObservableObject
{
    [ObservableProperty]
    public object _viewModel = null!;
}
