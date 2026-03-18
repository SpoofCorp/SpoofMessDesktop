using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;
using System.Collections.ObjectModel;

namespace SpoofMess.ViewModels.FileViewModels;

public partial class FileViewModel : ObservableObject
{
    public ObservableCollection<FileObject> Files { get; set; } = [];
}
