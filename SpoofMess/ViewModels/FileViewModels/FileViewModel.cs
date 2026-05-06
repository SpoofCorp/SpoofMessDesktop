namespace SpoofMess.ViewModels.FileViewModels;

public partial class FileViewModel : ObjectViewModel
{
    public override string Icon { get; init; } = "📁";

    public override ObjectViewModel Clone() =>
        new FileViewModel() { Files = [.. Files], Icon = Icon };
}
