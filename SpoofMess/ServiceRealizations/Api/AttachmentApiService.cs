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
    protected override string BaseUrl => "https://localhost:7146/api/Attachment";

    public async Task<Result<FileMetadata>> GetToken(byte[] token) =>
        await PostAsync<byte[], FileMetadata>($"/get-token", token);
}
