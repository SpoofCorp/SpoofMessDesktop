using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.Wave;
using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.FileViewModels;

public partial class MusicViewModel : ObjectViewModel, IDisposable
{
    private readonly IAudioService _audioService;

    [ObservableProperty]
    public partial PlayableFileObject? CurrentFile { get; set; }

    public override ObjectViewModel Clone() =>
        new MusicViewModel(_audioService) { Files = [.. Files], Icon = Icon };
    public override string Icon { get; init; } = "🎶";

    public MusicViewModel(IAudioService audioService)
    {
        _audioService = audioService;
        _audioService.OutputDevice_PlaybackStopped += OutputDevice_PlaybackStopped;
    }

    [RelayCommand]
    private void PlayResume(FileObject file)
    {
        if (file is not PlayableFileObject playableFile)
            return;
        if (playableFile != CurrentFile)
        {
            if (CurrentFile != null)
            {
                _audioService.Stop();
                CurrentFile.IsPlayed = false;
            }
            if (file.Path is not null && System.IO.File.Exists(file.Path))
                _audioService.Play(playableFile);

            CurrentFile = playableFile;
        }
        else
        {
            _audioService.PauseResume();
        }
    }

    public override void Add(FileObject fileObject) =>
        Files.Add(new PlayableFileObject(fileObject));

    public void Dispose()
    {
        _audioService.OutputDevice_PlaybackStopped -= OutputDevice_PlaybackStopped;
        GC.SuppressFinalize(this);
    }
    public void OutputDevice_PlaybackStopped(object? sender, StoppedEventArgs e)
    {
        CurrentFile?.IsPlayed = false;
    }
}
