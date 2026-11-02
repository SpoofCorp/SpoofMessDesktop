using CommonObjects.Results;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using NAudio.MediaFoundation;
using NAudio.Wave;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.ViewModels.FileViewModels;
using System.Collections.ObjectModel;

namespace SpoofMess.ViewModels.Notifications;

public partial class MusicNotification(
        IAudioService audioService,
        INotificationService notificationService,
        IAppLifeCycleService appLifeCycleService
    ) : AdditionalViewModel(appLifeCycleService)
{
    public IAudioService AudioService { get; set; } = audioService;
    private readonly INotificationService _notificationService = notificationService;
    public ObservableCollection<PlayableFileObject> SongsList { get; private set; } = [];

    [ObservableProperty]
    public partial PlayableFileObject? CurrentFile { get; set; }

    [ObservableProperty]
    public partial MusicViewModel? CurrentMusicViewModel { get; set; }
    private bool _isActiveViewModel = false;
    private int currentIndex = 0;

    [RelayCommand]
    public void PlayResume()
    {
        if (CurrentFile is not null)
            PlayResume(CurrentFile);
    }

    [RelayCommand(CanExecute = nameof(CanScrollRight))]
    public void NextSong()
    {
        if (CurrentFile is null) return;

        currentIndex++;
        PlayResume(SongsList[currentIndex]);
    }

    [RelayCommand(CanExecute = nameof(CanScrollLeft))]
    public void PreviewSong()
    {
        if (CurrentFile is null) return;

        currentIndex--;
        PlayResume(SongsList[currentIndex]);
    }

    public void PlayResume(PlayableFileObject playableFile)
    {
        if (playableFile != CurrentFile)
        {
            CurrentFile?.IsPlayed = false;
            if (playableFile.Path is not null && System.IO.File.Exists(playableFile.Path))
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
                CurrentFile = playableFile;
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
    protected override async Task InitializeAsync(bool initialize)
    {
        await AudioService.InitializeAsync();
        AudioService.OutputDevice_PlaybackStopped += OutputDevice_PlaybackStopped;
        ServiceRealizations.EventHandler.OnChangeStaticNotificationViewModel += OnChange;
        await base.InitializeAsync(initialize);
    }

    protected override async ValueTask DisposeAsync(bool disposing)
    {
        ServiceRealizations.EventHandler.OnChangeStaticNotificationViewModel -= OnChange;
        AudioService.OutputDevice_PlaybackStopped -= OutputDevice_PlaybackStopped;
        await AudioService.DisposeAsync();
        await base.DisposeAsync(disposing);
    }
    private void OnChange(BaseViewModel? baseViewModel)
    {
        if (baseViewModel == this)
            return;
        CurrentMusicViewModel = null;
        CurrentFile = null;
        _isActiveViewModel = false;
        AudioService.Stop();
    }
    public void OutputDevice_PlaybackStopped(object? sender, StoppedEventArgs e)
    {
        CurrentMusicViewModel?.CurrentFile?.IsPlayed = false;
        CurrentFile?.IsPlayed = false;
        if (CanScrollRight())
            NextSong();
    }
    public bool CanScrollLeft() =>
        CurrentFile is not null && CanScroll(currentIndex - 1);

    public bool CanScrollRight() =>
        CurrentFile is not null && CanScroll(currentIndex + 1);

    public bool CanScroll(int value) =>
        SongsList.Count > 0 && SongsList.Count > value && value >= 0;

    public void AddSong(PlayableFileObject playableFileObject)
    {
        SongsList.Add(playableFileObject);
        PreviewSongCommand.NotifyCanExecuteChanged();
        NextSongCommand.NotifyCanExecuteChanged();
    }

    public void AddSongWithIndex(PlayableFileObject playableFileObject)
    {
        currentIndex = SongsList.Count;
        AddSong(playableFileObject);
    }
}
