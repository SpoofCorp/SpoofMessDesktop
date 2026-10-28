using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Bases;
using SpoofMess.Services;
using System.Windows;

namespace SpoofMess.ViewModels;

public abstract partial class PlacementViewModel(IAppLifeCycleService appLifeCycleService) : BaseViewModel(appLifeCycleService), IPlacementViewModel
{
    [ObservableProperty]
    [NotifyPropertyChangedFor(nameof(AdditionalViewModelVisibility))]
    public partial AdditionalViewModel? AdditionalViewModel { get; set; }

    public Visibility AdditionalViewModelVisibility => AdditionalViewModel is null ? Visibility.Collapsed : Visibility.Visible;


    public virtual void ShowAdditionalViewModel(AdditionalViewModel additionalViewModel)
    {
        AdditionalViewModel = additionalViewModel;
        additionalViewModel.Initialize(this, CloseAdditionalViewModel);
    }

    protected virtual void CloseAdditionalViewModel() =>
        AdditionalViewModel = null;

    public void BehindCloseAdditionalViewModel()
    {
        AdditionalViewModel?.CloseCommand.Execute(null);
    }
}