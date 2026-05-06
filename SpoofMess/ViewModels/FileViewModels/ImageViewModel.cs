using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;

namespace SpoofMess.ViewModels.FileViewModels;

public partial class ImageViewModel : ObjectViewModel
{
    public override string Icon { get; init; } = "🖼️";

    public override ObjectViewModel Clone() =>
        new ImageViewModel() { Files = [.. Files], Icon = Icon };

    [RelayCommand]
    private void Zoom(FileObject file)
    {

    }
}
