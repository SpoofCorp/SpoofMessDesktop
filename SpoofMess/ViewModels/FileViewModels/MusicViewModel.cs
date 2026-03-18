using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.Wave;
using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.FileViewModels;

public partial class MusicViewModel : FileViewModel, IDisposable
{
    private readonly IAudioService _audioService;
    private bool _init;
    [ObservableProperty]
    private bool _isPlayeed;

    public MusicViewModel(IAudioService audioService)
    {
        _audioService = audioService;
        _audioService.OutputDevice_PlaybackStopped += OutputDevice_PlaybackStopped;
    }

    [RelayCommand]
    private void PlayResume(FileObject file)
    {
        if (_init)
            _audioService.PauseResume();
        else if (file.Path is not null && System.IO.File.Exists(file.Path))
        {
            _audioService.Play(file.Path);
            _init = true;
        }
        IsPlayeed = _audioService.IsPlayeed;
    }

    public void Dispose()
    {
        _audioService?.OutputDevice_PlaybackStopped -= OutputDevice_PlaybackStopped;
        GC.SuppressFinalize(this);
    }
    public void OutputDevice_PlaybackStopped(object? sender, StoppedEventArgs e)
    {
        IsPlayeed = _init = false;
    }
}
