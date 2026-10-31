using SpoofMess.Models;

namespace SpoofMess.Services;

public interface IAudioRecordService
{
    public Task Record();
    public Task Stop();
    public void ChangeDevice(AudioInDevice? audioInDevice);
}
