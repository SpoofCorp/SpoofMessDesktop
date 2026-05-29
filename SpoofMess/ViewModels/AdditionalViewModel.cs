using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;

namespace SpoofMess.ViewModels;

public abstract partial class AdditionalViewModel : ObservableObject
{
    protected ObservableObject _ownerView = null!;
    protected Action _close = null!;


    [RelayCommand]
    protected void Close()
    {
        _close();
        OnClose();
    }

    public virtual void Initialize(ObservableObject ownerView, Action close)
    {
        _ownerView = ownerView;
        _close = close;
    }

    public virtual void OnClose()
    {

    }
}
