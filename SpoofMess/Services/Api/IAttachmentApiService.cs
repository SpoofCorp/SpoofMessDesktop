using CommonObjects.DTO;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface IAttachmentApiService
{
    public Task<Result<FileMetadata>> GetToken(byte[] token);
}
