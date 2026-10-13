using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.Wave;
using SpoofMess.Models;
using SpoofMess.Services;
using System.IO;

namespace SpoofMess.ServiceRealizations;

public partial class AudioService : ObservableObject, IAudioService
{
    [ObservableProperty]
    public partial PlayableFileObject? PlayableFileObject { get; set; }

    private AudioFileReader _reader = null!;
    private WaveOutEvent _outputDevice = null!;
    public bool IsPlayeed { get; set; }
    public bool IsEnded { get; set; }
    public EventHandler<StoppedEventArgs> OutputDevice_PlaybackStopped { get; set; } = null!;

    public void Play(PlayableFileObject fileObject)
    {
        PlayableFileObject = fileObject;
        if (!File.Exists(fileObject.Path))
            return;

        if (_outputDevice is null)
        {
            _outputDevice = new();
            _outputDevice.PlaybackStopped += OnStoped;
        }
        else
            Stop();

        IsEnded = false;
        _reader?.Dispose();
        _reader = new(fileObject.Path);
        _outputDevice.Init(_reader);
        _outputDevice.Play();
        PlayableFileObject.IsPlayed = IsPlayeed = true;
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

    public void Stop()
    {
        _outputDevice.Stop();
        IsEnded = true;
        PlayableFileObject?.IsPlayed = IsPlayeed = false;
    }

    protected void OnStoped(object? sender, StoppedEventArgs args)
    {
        Stop();
        OutputDevice_PlaybackStopped(sender, args);
    }

    public bool ItIsPlayeed(PlayableFileObject fileObject) =>
        PlayableFileObject == fileObject;
}
