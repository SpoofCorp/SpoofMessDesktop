using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;

namespace SpoofMess.ViewModels.FileViewModels;

public partial class FileViewModel : ObservableObject
{
    public FileObject File { get; set; } = null!;
}
