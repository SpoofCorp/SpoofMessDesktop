using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;
using System.Collections.ObjectModel;

namespace SpoofMess.ViewModels.FileViewModels;

public abstract partial class ObjectViewModel : ObservableObject
{
    public ObservableCollection<FileObject> Files { get; set; } = [];
    public abstract string Icon { get; init; }

    public abstract ObjectViewModel Clone();

    public virtual void Add(FileObject fileObject) =>
        Files.Add(fileObject);
}
