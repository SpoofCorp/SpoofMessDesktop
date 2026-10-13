using AdditionalHelpers.Services;
using CommonObjects.Requests.Files;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.IO;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

public class FileApiService(
    HttpClient client,
    ISerializer serializer) : ApiService(
        client, 
        serializer), IFileApiService
{
    public async Task<Result> Save()
    {
        throw new NotImplementedException();
    }
    public async Task<Result<Stream>> Upload(byte[] token, CancellationToken ct = default)
    {
        return await PostStreamAsync("api/v3/File/upload", token, ct);
    }


    public async Task<Result> ExistsL1L2(FingerprintExistL1L2 l1L2, CancellationToken token = default)
    {
        return await PostAsync("api/v3/File/first-check", l1L2, token);
    }

    public async Task<Result<byte[]>> ExistsL3(FingerprintExistL3 l3, CancellationToken token = default)
    {
        return await PostAsync<FingerprintExistL3, byte[]>("api/v3/File/full-check", l3, token);
    }

    public async Task<Result<byte[]>> Save(MultipartFormDataContent content, CancellationToken token = default)
    {
        return await PostAsync<byte[]>("api/v3/File/save", content, token);
    }


    public async Task<Result<CommonObjects.DTO.FileMetadata>> GetToken(Guid fileId, CancellationToken token = default)
    {
        return await GetAsync<CommonObjects.DTO.FileMetadata>($"api/v3/File/get?fileId={fileId}", token);
    }

}
