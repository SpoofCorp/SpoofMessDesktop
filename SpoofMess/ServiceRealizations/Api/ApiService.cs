using AdditionalHelpers.Services;
using CommonObjects.Results;
using System.IO;
using System.Net.Http;
using System.Net.Http.Json;
using System.Text;

namespace SpoofMess.ServiceRealizations.Api;

public abstract class ApiService(HttpClient client, ISerializer serializer) : IDisposable
{
    protected readonly ISerializer _serializer = serializer;
    private readonly HttpClient _client = client;
    private readonly CancellationTokenSource _cts = new();
    protected abstract string BaseUrl { get; }


    protected virtual async Task<Result> DeleteAsync(string requestUrl, CancellationToken token = default)
    {
        try
        {
            HttpResponseMessage response = await _client.DeleteAsync(
                    GetUrl(requestUrl),
                    token == default ? _cts.Token : token
                );
            return Result.Parse(
                await response.Content.ReadAsStringAsync(token == default ? _cts.Token : token),
                (int)response.StatusCode
            );
        }
        catch
        {
            return Result.ErrorResult("");
        }
    }


    protected virtual async Task<Result> GetAsync(string requestUrl,  CancellationToken token = default)
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync(
                    GetUrl(requestUrl),
                    token == default ? _cts.Token : token
                );
            return Result.Parse(
                await response.Content.ReadAsStringAsync(token == default ? _cts.Token : token),
                (int)response.StatusCode
            );
        }
        catch
        {
            return Result.ErrorResult("");
        }
    }

    protected virtual async Task<Result<TResult>> GetAsync<TResult>(string requestUrl,  CancellationToken token = default)
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync(
                    GetUrl(requestUrl),
                    token == default ? _cts.Token : token
                );
            return await Parse<TResult>(response, token);
        }
        catch
        {
            return Result<TResult>.ErrorResult("");
        }
    }

    protected virtual async Task<Result> PostAsync<T>(string requestUrl, T obj,  CancellationToken token = default)
    {
        return await PostAsync(
                requestUrl: requestUrl,
                content: new StringContent(
                    _serializer.Serialize(obj),
                    Encoding.UTF8,
                    "application/json"
                    ),
                token: token == default ? _cts.Token : token
            );
    }
    protected virtual async Task<Result<TResult>> PostAsync<T, TResult>(string requestUrl, T obj,  CancellationToken token = default)
    {
        try
        {
            HttpResponseMessage response = await _client.PostAsync(
                    GetUrl(requestUrl),
                    new StringContent(
                            _serializer.Serialize(obj),
                            Encoding.UTF8,
                            "application/json"
                        ),
                    token == default ? _cts.Token : token
                );
            return await Parse<TResult>(response, token);
        }
        catch
        {
            return Result<TResult>.ErrorResult("");
        }
    }


    protected virtual async Task<Result> PostAsync(string requestUrl, HttpContent content,  CancellationToken token = default)
    {
        try
        {
            HttpResponseMessage response = await _client.PostAsync(
                    GetUrl(requestUrl),
                    content,
                    token == default ? _cts.Token : token
                );
            return Result.Parse(
                        await response.Content.ReadAsStringAsync(token == default ? _cts.Token : token),
                        (int)response.StatusCode
                    );
        }
        catch
        {
            return Result.ErrorResult("");
        }
    }
    protected virtual async Task<Result<TResult>> PostAsync<TResult>(string requestUrl, HttpContent content,  CancellationToken token = default)
    {
        try
        {
            HttpResponseMessage response = await _client.PostAsync(
                    GetUrl(requestUrl),
                    content,
                    token == default ? _cts.Token : token
                );
            return await Parse<TResult>(response, token);
        }
        catch
        {
            return Result<TResult>.ErrorResult("");
        }
    }
    protected virtual async Task<Result> PatchAsync<T>(string requestUrl, T obj, CancellationToken token = default)
    {
        try
        {
            HttpResponseMessage response = await _client.PatchAsJsonAsync(
                    GetUrl(requestUrl),
                    obj,
                    token == default ? _cts.Token : token
                ); 
            return Result.Parse(
                        await response.Content.ReadAsStringAsync(token == default ? _cts.Token : token),
                        (int)response.StatusCode
                    );
        }
        catch
        {
            return Result.ErrorResult("");
        }
    }

    public void Dispose()
    {
        _cts.Cancel();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
    protected virtual async Task<Result<Stream>> PostStreamAsync<T>(string requestUrl, T obj,  CancellationToken token = default)
    {
        return await PostStreamAsync(
                requestUrl,
                content: new StringContent(
                    _serializer.Serialize(obj),
                    Encoding.UTF8,
                    "application/json"
                    ),
                token == default ? _cts.Token : token
            );
    }

    protected virtual async Task<Result<Stream>> PostStreamAsync(string requestUrl, HttpContent content,  CancellationToken token = default)
    {
        try
        {
            HttpRequestMessage request = new(HttpMethod.Post, GetUrl(requestUrl))
            {
                Content = content
            };
            HttpResponseMessage response = await _client.SendAsync(
                    request,
                    HttpCompletionOption.ResponseHeadersRead,
                    token == default ? _cts.Token : token
                );
            if (response.IsSuccessStatusCode)
                return Result<Stream>.Parse(
                        "",
                        await response.Content.ReadAsStreamAsync(token == default ? _cts.Token : token),
                        (int)response.StatusCode
                    );
            else
                return Result<Stream>.Parse(
                        await response.Content.ReadAsStringAsync(token == default ? _cts.Token : token),
                        default,
                        (int)response.StatusCode
                    );
        }
        catch
        {
            return Result<Stream>.ErrorResult("");
        }

    }

    private async Task<Result<T>> Parse<T>(HttpResponseMessage response,  CancellationToken token = default)
    {
        if (response.IsSuccessStatusCode)
            return Result<T>.Parse(
                 "",
                 await _serializer.Deserialize<T>(
                        await response.Content.ReadAsStreamAsync(token == default ? _cts.Token : token)
                     ),
                 (int)response.StatusCode
             );
        else
            return Result<T>.Parse(
                await response.Content.ReadAsStringAsync(token == default ? _cts.Token : token),
                default,
                (int)response.StatusCode
            );
    }
    protected string GetUrl(string url) => $"{BaseUrl}{url}";
}