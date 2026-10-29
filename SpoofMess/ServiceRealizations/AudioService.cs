using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.Wave;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using System.IO;

namespace SpoofMess.ServiceRealizations;

public partial class AudioService(IAppLifeCycleService appLifeCycleService) : BaseViewModel(appLifeCycleService), IAudioService
{
    [ObservableProperty]
    public partial PlayableFileObject? PlayableFileObject { get; set; }

    private AudioFileReader _reader = null!;
    private WaveOut _outputDevice = null!;
    public bool IsPlayeed { get; set; }
    public bool IsEnded { get; set; }
    public EventHandler<StoppedEventArgs> OutputDevice_PlaybackStopped { get; set; } = null!;

    public void Play(PlayableFileObject fileObject)
    {
        _outputDevice?.Stop();

        PlayableFileObject?.IsPlayed = false;
        PlayableFileObject = fileObject;
        if (!File.Exists(fileObject.Path))
            return;

        if (_outputDevice is null)
        {
            _outputDevice = new();
            _outputDevice.PlaybackStopped += OnStoped;
        }

        IsEnded = false;
        _reader?.Dispose();
        _reader = new(fileObject.Path);
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

        if (PlayableFileObject is not null)
        {
            PlayableFileObject.IsPlayed = false;
            PlayableFileObject = null;
        }
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

    protected override ValueTask DisposeAsync(bool disposing)
    {
        _outputDevice.PlaybackStopped -= OnStoped;
        return base.DisposeAsync(disposing);
    }
}