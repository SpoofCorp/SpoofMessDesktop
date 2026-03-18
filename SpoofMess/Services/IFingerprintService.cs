using CommonObjects.Requests.Files;

namespace SpoofMess.Services;

public interface IFingerprintService
{
    public byte[] GetFingerPrintL1(string filePath);
    public byte[] GetFingerPrintL2(string filePath);
    public Task<FingerprintExistL3> GetFingerPrintFull(string filePath, CancellationToken token);
    public FingerprintExistL1L2 GetFingerPrintL1L2(string filePath);
}
