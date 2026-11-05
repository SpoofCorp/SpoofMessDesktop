using NAudio.MediaFoundation;
using NAudio.Wave;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ServiceRealizations;

public partial class AudioRecorderService(
    AppSessionSettings appSessionSettings) : IAudioRecordService, IInitializable, IDisposable
{
    protected AppSessionSettings _appSessionSettings = appSessionSettings;
    protected static readonly WaveFormat waveFormat = new(44100, 16, 1);
    protected WaveIn waveIn = null!;

    public bool IsInitialized { get; set; }

    public virtual void Initialize()
    {
        if (IsInitialized)
        {
            throw new ApplicationException("It's been initialized");
        }
        MediaFoundationApi.Startup();
        waveIn = new()
        {
            WaveFormat = waveFormat,
            DeviceNumber = _appSessionSettings.AudioInDevice?.Number ?? 0
        };

        waveIn.DataAvailable += WaveInDataAvailable;

        waveIn.RecordingStopped += OnRecordingStopped;
        IsInitialized = true;
    }

    protected virtual void WaveInDataAvailable(object? sender, WaveInEventArgs e)
    {

    }

    protected virtual void OnRecordingStopped(object? sender, StoppedEventArgs e)
    {
    }

    public virtual async Task Record()
    {
        waveIn.StartRecording();
    }

    public virtual async Task Stop()
    {
        waveIn.StopRecording();
    }

    public void Dispose()
    {

        waveIn.DataAvailable -= WaveInDataAvailable;

        waveIn.RecordingStopped -= OnRecordingStopped;
        waveIn.StopRecording();
        MediaFoundationApi.Shutdown();
        GC.SuppressFinalize(this);
    }

    public void ChangeDevice(AudioInDevice? audioInDevice)
    {
        if (audioInDevice is null) return;

        waveIn.DeviceNumber = audioInDevice.Number;
    }
}
