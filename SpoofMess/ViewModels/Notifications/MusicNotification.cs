using CommonObjects.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.MediaFoundation;
using NAudio.Wave;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.ViewModels.FileViewModels;

namespace SpoofMess.ViewModels.Notifications;

public partial class MusicNotification(
        IAudioService audioService,
        INotificationService notificationService,
        IAppLifeCycleService appLifeCycleService
    ) : AdditionalViewModel(appLifeCycleService)
{
    public IAudioService AudioService { get; set; } = audioService;
    private readonly INotificationService _notificationService = notificationService;
    [ObservableProperty]
    public partial MusicViewModel? CurrentMusicViewModel { get; set; }
    private bool _isActiveViewModel = false;

    [RelayCommand]
    public void PlayResume()
    {
        if (CurrentMusicViewModel is not null && CurrentMusicViewModel.CurrentFile is not null)
            PlayResume(CurrentMusicViewModel, CurrentMusicViewModel.CurrentFile);
    }

    [RelayCommand(CanExecute = nameof(CanScrollRight))]
    public void NextSong()
    {
        if (CurrentMusicViewModel is not null)
        {
            CurrentMusicViewModel.CurrentIndex++;
            PlayResume(CurrentMusicViewModel, CurrentMusicViewModel.Files[CurrentMusicViewModel.CurrentIndex]);
        }
    }

    [RelayCommand(CanExecute = nameof(CanScrollLeft))]
    public void PreviewSong()
    {
        if (CurrentMusicViewModel is not null)
        {
            CurrentMusicViewModel.CurrentIndex--;
            PlayResume(CurrentMusicViewModel, CurrentMusicViewModel.Files[CurrentMusicViewModel.CurrentIndex]);
        }
    }

    private void PlayResume(MusicViewModel musicViewModel, FileObject file)
    {
        if (file is not PlayableFileObject playableFile)
            return;

        if (playableFile != CurrentMusicViewModel?.CurrentFile)
        {
            musicViewModel.CurrentFile?.IsPlayed = false;
            if (file.Path is not null && System.IO.File.Exists(file.Path))
            {
                try
                {
                    AudioService.Play(playableFile);
                }
                catch (MediaFoundationException)
                {
                    _notificationService.ShowError(Result.ErrorResult("Unsupported audio format", 204));
                    return;
                }
                musicViewModel.CurrentFile = playableFile;
                musicViewModel.CurrentFile.IsPlayed = true;
                CurrentMusicViewModel = musicViewModel;
                _isActiveViewModel = true; 
                PreviewSongCommand.NotifyCanExecuteChanged();
                NextSongCommand.NotifyCanExecuteChanged();
                ServiceRealizations.EventHandler.NotifyChangeStaticNotificationViewModel(this);
            }
        }
        else
        {
            AudioService.PauseResume();
        }
    }
    protected override Task InitializeAsync(bool initialize)
    {
        AudioService.OutputDevice_PlaybackStopped += OutputDevice_PlaybackStopped;
        ServiceRealizations.EventHandler.OnChangeStaticNotificationViewModel += OnChange;
        ServiceRealizations.EventHandler.OnChangeMusicViewModel += PlayResume;
        return base.InitializeAsync(initialize);
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        ServiceRealizations.EventHandler.OnChangeStaticNotificationViewModel -= OnChange;
        AudioService.OutputDevice_PlaybackStopped -= OutputDevice_PlaybackStopped;
        await AudioService.DisposeAsync();
        ServiceRealizations.EventHandler.OnChangeMusicViewModel -= PlayResume;
        await base.DisposeAsync(disposing);
    }
    private void OnChange(BaseViewModel? baseViewModel)
    {
        if (baseViewModel == this)
            return;
        CurrentMusicViewModel = null;
        _isActiveViewModel = false;
        AudioService.Stop();
    }
    public void OutputDevice_PlaybackStopped(object? sender, StoppedEventArgs e)
    {
        CurrentMusicViewModel?.CurrentFile?.IsPlayed = false;
    }

    public bool CanScrollLeft() =>
        CurrentMusicViewModel is not null && CurrentMusicViewModel.CanScroll(CurrentMusicViewModel.CurrentIndex - 1);

    public bool CanScrollRight() =>
        CurrentMusicViewModel is not null && CurrentMusicViewModel.CanScroll(CurrentMusicViewModel.CurrentIndex + 1);
}
