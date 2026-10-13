using CommunityToolkit.Mvvm.ComponentModel;
using System.Windows;

namespace SpoofMess.ViewModels;

public abstract partial class PlacementAdditionalViewModel : AdditionalViewModel, IPlacementViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AdditionalViewModelVisibility))]
    [NotifyPropertyChangedFor(nameof(MainVisibility))]
    public partial AdditionalViewModel? AdditionalViewModel { get; set; }

    public Visibility AdditionalViewModelVisibility => AdditionalViewModel is null ? Visibility.Collapsed : Visibility.Visible;

    public Visibility MainVisibility => AdditionalViewModel is null ? Visibility.Visible : Visibility.Collapsed;

    public virtual void ShowAdditionalViewModel(AdditionalViewModel additionalViewModel)
    {
        AdditionalViewModel = additionalViewModel;
        additionalViewModel.Initialize(this, CloseAdditionalViewModel);
    }

    public virtual void ShowAdditionalViewModel(AdditionalViewModel additionalViewModel, AdditionalViewModel predecessor)
    {
        AdditionalViewModel = additionalViewModel;
        additionalViewModel.Initialize(this, predecessor, CloseAdditionalViewModel);
    }

    protected virtual void CloseAdditionalViewModel() =>
        AdditionalViewModel = null;
}