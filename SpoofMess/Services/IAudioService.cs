using NAudio.Wave;
using SpoofMess.Bases;
using SpoofMess.Models;

namespace SpoofMess.Services;

public interface IAudioService : IAsyncInitializable, IAsyncDisposable
{
    public PlayableFileObject? PlayableFileObject { get; set; }
    public EventHandler<StoppedEventArgs> OutputDevice_PlaybackStopped { get; set; }
    public bool IsPlayeed { get; set; }
    public bool ItIsPlayeed(PlayableFileObject fileObject);
    public void Play(PlayableFileObject fileObject);
    public void PauseResume();
    public void Stop();
}
