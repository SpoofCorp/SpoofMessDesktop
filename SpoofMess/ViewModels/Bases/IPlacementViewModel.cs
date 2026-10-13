using System.Windows;

namespace SpoofMess.ViewModels;

public interface IPlacementViewModel
{
    public AdditionalViewModel? AdditionalViewModel { get; set; }

    public Visibility AdditionalViewModelVisibility { get; }
    public void ShowAdditionalViewModel(AdditionalViewModel additionalViewModel);
}
