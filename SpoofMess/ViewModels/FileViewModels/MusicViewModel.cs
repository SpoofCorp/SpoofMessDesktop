using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;

namespace SpoofMess.ViewModels.FileViewModels;

public partial class MusicViewModel : ObjectViewModel
{
    [ObservableProperty]
    public partial PlayableFileObject? CurrentFile { get; set; }

    public override ObjectViewModel Clone() =>
        new MusicViewModel { Files = [.. Files], Icon = Icon };
    public override string Icon { get; init; } = "🎶";

    [RelayCommand]
    private void PlayResume(FileObject file)
    {
        ServiceRealizations.EventHandler.NotifyChangeMusicViewModel(this, file);
    }

    public override void Add(FileObject fileObject) =>
        Files.Add(new PlayableFileObject(fileObject));

 }
