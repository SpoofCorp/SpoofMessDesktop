using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;

namespace SpoofMess.ViewModels;

public abstract partial class AdditionalViewModel : BaseViewModel
{
    public AdditionalViewModel? Predecessor;
    protected IPlacementViewModel _placement = null!;
    protected Action _close = null!;


    [RelayCommand]
    protected virtual void Close()
    {
        _close();
        OnClose();
    }

    public virtual void Initialize(IPlacementViewModel ownerView, Action close)
    {
        _placement = ownerView;
        _close = close;
    }

    public virtual void Initialize(IPlacementViewModel placement, AdditionalViewModel predecessor, Action close)
    {
        _placement = placement;
        _close = close;
        Predecessor = predecessor;
    }

    public virtual void OnClose()
    {

    }
}