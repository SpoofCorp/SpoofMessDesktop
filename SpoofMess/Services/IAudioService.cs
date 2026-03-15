using NAudio.Wave;

namespace SpoofMess.Services;

public interface IAudioService
{
    public EventHandler<StoppedEventArgs> OutputDevice_PlaybackStopped { get; set; }
    public bool IsPlayeed { get; set; }
    public void Play(string path);
    public void PauseResume();
    public void Stop();
}
