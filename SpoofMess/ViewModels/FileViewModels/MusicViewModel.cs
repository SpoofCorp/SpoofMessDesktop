using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.Wave;
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
    private void PlayResume()
    {
        if (_init)
            _audioService.PauseResume();
        else if (File.Path is not null && System.IO.File.Exists(File.Path))
        {
            _audioService.Play(File.Path);
            _init = true;
        }
        IsPlayeed = _audioService.IsPlayeed;
    }
    public void OutputDevice_PlaybackStopped(object? sender, StoppedEventArgs e)
    {
        IsPlayeed = _init = false;
    }

    public void Dispose()
    {
        if (_audioService is not null && _audioService.OutputDevice_PlaybackStopped is not null)
            _audioService.OutputDevice_PlaybackStopped -= OutputDevice_PlaybackStopped;
        GC.SuppressFinalize(this);
    }
}
