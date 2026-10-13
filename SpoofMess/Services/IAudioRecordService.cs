namespace SpoofMess.Services;

internal interface IAudioRecordService
{
    public Task Record();
    public Task Stop();
}
