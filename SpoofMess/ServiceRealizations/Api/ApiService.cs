using AdditionalHelpers.Services;
using CommonObjects.Results;
using System.IO;
using System.Net.Http;
using System.Text;

namespace SpoofMess.ServiceRealizations.Api;

public abstract class ApiService(HttpClient client, ISerializer serializer) : IDisposable
{
    protected readonly ISerializer _serializer = serializer;
    private readonly HttpClient _client = client;
    private readonly CancellationTokenSource _cts = new();
    protected abstract string BaseUrl { get; }

    protected virtual async Task<HttpResponseMessage> DeleteAsync(string requestUrl)
    {
        throw new NotImplementedException();
    }

    protected virtual async Task<HttpResponseMessage> DeleteAsync<T>(string requestUrl, T obj) where T : class
    {
        throw new NotImplementedException();
    }


    protected virtual async Task<Result> GetAsync(string requestUrl, CancellationToken? token = null)
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync(
                    GetUrl(requestUrl),
                    token ?? _cts.Token
                );
            return Result.Parse(
                await response.Content.ReadAsStringAsync(token ?? _cts.Token),
                (int)response.StatusCode
            );
        }
        catch
        {
            return Result.ErrorResult("");
        }
    }

    protected virtual async Task<Result<TResult>> GetAsync<TResult>(string requestUrl, CancellationToken? token = null)
    {
        try
        {
            HttpResponseMessage response = await _client.GetAsync(
                    GetUrl(requestUrl),
                    token ?? _cts.Token
                );
            return await Parse<TResult>(response, token);
        }
        catch
        {
            return Result<TResult>.ErrorResult("");
        }
    }

    protected virtual async Task<Result> PostAsync<T>(string requestUrl, T obj, CancellationToken? token = null)
    {
        return await PostAsync(
                requestUrl,
                new StringContent(
                    _serializer.Serialize(obj),
                    Encoding.UTF8,
                    "application/json"
                    ),
                token ?? _cts.Token
            );
    }
    protected virtual async Task<Result<TResult>> PostAsync<T, TResult>(string requestUrl, T obj, CancellationToken? token = null)
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
                    token ?? _cts.Token
                );
            return await Parse<TResult>(response, token);
        }
        catch
        {
            return Result<TResult>.ErrorResult("");
        }
    }


    protected virtual async Task<Result> PostAsync(string requestUrl, HttpContent content, CancellationToken? token = null)
    {
        try
        {
            HttpResponseMessage response = await _client.PostAsync(
                    GetUrl(requestUrl),
                    content,
                    token ?? _cts.Token
                );
            return Result.Parse(
                        await response.Content.ReadAsStringAsync(token ?? _cts.Token),
                        (int)response.StatusCode
                    );
        }
        catch
        {
            return Result.ErrorResult("");
        }
    }
    protected virtual async Task<Result<TResult>> PostAsync<TResult>(string requestUrl, HttpContent content, CancellationToken? token = null)
    {
        try
        {
            HttpResponseMessage response = await _client.PostAsync(
                    GetUrl(requestUrl),
                    content,
                    token ?? _cts.Token
                );
            return await Parse<TResult>(response, token);
        }
        catch
        {
            return Result<TResult>.ErrorResult("");
        }
    }

    protected virtual async Task<HttpResponseMessage> PatchAsync(string requestUrl, CancellationToken? token = null)
    {
        throw new NotImplementedException();
    }
    public void Dispose()
    {
        _cts.Cancel();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
    protected virtual async Task<Result<Stream>> PostStreamAsync<T>(string requestUrl, T obj, CancellationToken? token = null)
    {
        return await PostStreamAsync(
                requestUrl,
                content: new StringContent(
                    _serializer.Serialize(obj),
                    Encoding.UTF8,
                    "application/json"
                    ),
                token ?? _cts.Token
            );
    }

    protected virtual async Task<Result<Stream>> PostStreamAsync(string requestUrl, HttpContent content, CancellationToken? token = null)
    {
        try
        {
            HttpResponseMessage response = await _client.PostAsync(
                    GetUrl(requestUrl),
                    content,
                    token ?? _cts.Token
                );
            if( response.IsSuccessStatusCode )
            return Result<Stream>.Parse(
                    "",
                    await response.Content.ReadAsStreamAsync(token ?? _cts.Token),
                    (int)response.StatusCode
                );
            else
                return Result<Stream>.Parse(
                        await response.Content.ReadAsStringAsync(token ?? _cts.Token),
                        null,
                        (int)response.StatusCode
                    );
        }
        catch
        {
            return Result<Stream>.ErrorResult("");
        }

    }

    private async Task<Result<T>> Parse<T>(HttpResponseMessage response, CancellationToken? token = null)
    {
        if (response.IsSuccessStatusCode)
            return Result<T>.Parse(
                 "",
                 await _serializer.Deserialize<T>(
                        await response.Content.ReadAsStreamAsync(token ?? _cts.Token)
                     ),
                 (int)response.StatusCode
             );
        else
            return Result<T>.Parse(
                await response.Content.ReadAsStringAsync(token ?? _cts.Token),
                default,
                (int)response.StatusCode
            );
    }
    protected string GetUrl(string url) => $"{BaseUrl}{url}";
}
