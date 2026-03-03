namespace SpoofMess.Services;

public interface IFingerprintService
{
    public Task<byte[]> GetFingerPrintL1(string filePath);
    public Task<byte[]> GetFingerPrintL2(string filePath);
    public Task<byte[]> GetFingerPrintFull(string filePath);
}
