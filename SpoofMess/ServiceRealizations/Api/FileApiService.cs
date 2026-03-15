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
    protected override string BaseUrl => "https://localhost:7138/api/v3/File";

    public async Task<Result> Save()
    {
        throw new NotImplementedException();
    }
    public async Task<Result<Stream>> Upload(byte[] token)
    {
        return await PostStreamAsync("/upload", token);
    }


    public async Task<Result> ExistsL1L2(FingerprintExistL1L2 l1L2)
    {
        return await PostAsync("/first-check", l1L2);
    }

    public async Task<Result<byte[]>> ExistsL3(FingerprintExistL3 l3)
    {
        return await PostAsync<FingerprintExistL3, byte[]>("/full-check", l3);
    }

    public async Task<Result<byte[]>> Save(MultipartFormDataContent content)
    {
        return await PostAsync<byte[]>("/save", content);
    }
}
