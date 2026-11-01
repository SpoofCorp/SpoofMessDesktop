using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.MediaFoundation;
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
            field = value < 0.0f ? 0.0f : value > 1.0f ? 1.0f : value;
            _outputDevice?.Volume = value;
            _userInfo.Volume = value;
            OnPropertyChanged();
        }
    }

    private AudioFileReader _reader = null!;
    private WaveOut _outputDevice = null!;
    public bool IsPlayeed { get; set; }
    public bool IsEnded { get; set; }
    public EventHandler<StoppedEventArgs> OutputDevice_PlaybackStopped { get; set; } = null!;
    public bool IsInitialized { get; set; }

    public void Play(PlayableFileObject fileObject)
    {
        _outputDevice.Stop();

        PlayableFileObject?.IsPlayed = false;
        PlayableFileObject = fileObject;
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
        _outputDevice.Play();
        PlayableFileObject.IsPlayed = IsPlayeed = true;
    }

    public void Stop()
    {
        if (_outputDevice is null) return;

        _outputDevice.Stop();
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
        if (!IsPlayeed)
        {
            IsEnded = true;
            PlayableFileObject?.IsPlayed = false;
            PlayableFileObject = null;
        }
    }

    public bool ItIsPlayeed(PlayableFileObject fileObject) =>
        PlayableFileObject == fileObject;

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