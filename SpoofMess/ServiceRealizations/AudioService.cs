using NAudio.Wave;
using SpoofMess.Services;
using System.IO;

namespace SpoofMess.ServiceRealizations;

public class AudioService : IAudioService
{
    private AudioFileReader _reader = null!;
    private WaveOutEvent _outputDevice = null!;
    public bool IsPlayeed { get; set; }
    public EventHandler<StoppedEventArgs> OutputDevice_PlaybackStopped { get; set; }

    public void Play(string path)
    {
        if (!File.Exists(path))
            return;

        if (_outputDevice is null)
        {
            _outputDevice = new();
            _outputDevice.PlaybackStopped += OutputDevice_PlaybackStopped;
        }

        _reader?.Dispose();
        _reader = new(path);
        _outputDevice.Init(_reader);
        _outputDevice.Play();
        IsPlayeed = true;
    }
    public void PauseResume()
    {
        if (IsPlayeed)
            _outputDevice.Pause();
        else
            _outputDevice.Play();
        IsPlayeed = !IsPlayeed;
    }

    public void Stop() =>
        _outputDevice.Stop();
}
