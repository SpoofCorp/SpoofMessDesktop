using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

internal class AttachmentApiService(
    HttpClient client,
    ISerializer serializer) : ApiService(
        client,
        serializer), IAttachmentApiService
{
    public async Task<Result<FileMetadata>> GetToken(byte[] token) =>
        await PostAsync<byte[], FileMetadata>("api/Attachment/get-token", token);
}
