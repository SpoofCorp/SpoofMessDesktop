using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.MediaFoundation;
using NAudio.Utils;
using NAudio.Wave;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using System.IO;

namespace SpoofMess.ServiceRealizations;

public partial class AudioService(UserInfo userInfo) : ObservableObject, IAudioService, IAsyncInitializable
{
    protected readonly UserInfo _userInfo = userInfo;
    [ObservableProperty]
    public partial PlayableFileObject? PlayableFileObject { get; set; }
    public float Volume
    {
        get;
        set
        {
            field = Math.Clamp(value, 0.0f, 1.0f);
            _outputDevice?.Volume = field;
            _userInfo.Volume = field;
            OnPropertyChanged();
        }
    }

    private AudioFileReader _reader = null!;
    private WaveOut _outputDevice = null!;
    public bool IsPlayeed { get; set; }
    public bool IsEnded { get; set; }
    public EventHandler<StoppedEventArgs> OutputDevice_PlaybackStopped { get; set; } = null!;
    public bool IsInitialized { get; set; }

    private bool isUserStoped = false;

    [ObservableProperty]
    public partial float TrackLenght { get; set; }

    public void Play(PlayableFileObject fileObject)
    {
        Stop(PlayableFileObject == fileObject);
        if (!File.Exists(fileObject.Path))
            return;

        IsEnded = false;
        
        _reader?.Dispose();
        try
        {
            _reader = new(fileObject.Path);
        }
        catch (MediaFoundationException)
        {
            PlayableFileObject = null;
            throw;
        }
        _outputDevice.Init(_reader);
        PlayableFileObject = fileObject;
        PlayableFileObject.IsPlayed = IsPlayeed = true;
        _outputDevice.Play();
    }

    public void Stop(bool isOldestFile = true, bool isUserStoped = true)
    {
        if (isOldestFile && (_outputDevice is null || !IsPlayeed)) return;

        _outputDevice.Stop();
        this.isUserStoped = isUserStoped;
        IsEnded = true;
        IsPlayeed = false;
        PlayableFileObject?.IsPlayed = false;
        PlayableFileObject = null;
    }

    public void PauseResume()
    {
        if (PlayableFileObject is null)
            return;
        if (IsPlayeed)
            _outputDevice.Pause();
        else
        {
            if (IsEnded)
            {
                _reader.Position = 0;
                IsEnded = false;
            }
            _outputDevice.Play();
        }
        PlayableFileObject.IsPlayed = IsPlayeed = !IsPlayeed;
    }

    protected void OnStoped(object? sender, StoppedEventArgs args)
    {
        if (!isUserStoped && IsPlayeed)
        {
            IsEnded = true;
            IsPlayeed = false;
            PlayableFileObject?.IsPlayed = false;
            OutputDevice_PlaybackStopped?.Invoke(sender, args);
        }
        isUserStoped = false;
    }

    public void SeekTo(TimeSpan position)
    {
        if (_reader == null || PlayableFileObject == null) return;

        position = TimeSpan.FromMilliseconds(Math.Clamp(position.TotalMilliseconds, 0, GetTotalTime().TotalMilliseconds));
        _reader.CurrentTime = position;

        PlayableFileObject.CurrentPosition = position;
    }

    public bool ItIsPlayeed(PlayableFileObject fileObject) =>
        PlayableFileObject == fileObject;

    public TimeSpan GetAccuratePosition() => _reader?.CurrentTime ?? TimeSpan.Zero;
    public TimeSpan GetTotalTime() => _reader?.TotalTime ?? TimeSpan.Zero;

    public Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        if (IsInitialized)
            return Task.CompletedTask;
        Volume = _userInfo.Volume;
        _outputDevice = new()
        {
            Volume = Volume
        };
        _outputDevice.PlaybackStopped += OnStoped;
        IsInitialized = true;
        return Task.CompletedTask;
    }

    public ValueTask DisposeAsync()
    {
        _reader?.Dispose();
        _outputDevice.PlaybackStopped -= OnStoped;
        _outputDevice.Dispose();
        GC.SuppressFinalize(this);
        return ValueTask.CompletedTask;
    }
}