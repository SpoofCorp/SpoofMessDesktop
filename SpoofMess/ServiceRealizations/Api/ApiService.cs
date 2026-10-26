using AdditionalHelpers.Services;
using CommonObjects.Results;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Net;
using System.Net.Http;
using System.Text;

namespace SpoofMess.ServiceRealizations.Api;

public abstract class ApiService(HttpClient client, ISerializer serializer) : IDisposable
{
    protected readonly ISerializer _serializer = serializer;
    protected readonly HttpClient _client = client;
    protected readonly CancellationTokenSource _cts = new();
    private readonly Version _defaultRequestVersion = HttpVersion.Version11;
    private readonly HttpVersionPolicy _defaultVersionPolicy = HttpVersionPolicy.RequestVersionOrLower;

    protected virtual async Task<Result> DeleteAsync([StringSyntax(StringSyntaxAttribute.Uri)] string requestUrl, CancellationToken token = default) =>
        await SendAsync(CreateRequestMessage(HttpMethod.Delete, CreateUri(requestUrl)), token: token);

    protected virtual async Task<Result> GetAsync([StringSyntax(StringSyntaxAttribute.Uri)] string requestUrl, CancellationToken token = default) =>
        await SendAsync(CreateRequestMessage(HttpMethod.Get, CreateUri(requestUrl)), token: token);

    protected virtual async Task<Result<TResult>> GetAsync<TResult>([StringSyntax(StringSyntaxAttribute.Uri)] string requestUrl, CancellationToken token = default) =>
        await SendAsync<TResult>(CreateRequestMessage(HttpMethod.Get, CreateUri(requestUrl)), token: token);

    protected virtual async Task<Result> PostAsync<T>(string requestUrl, T obj, CancellationToken token = default) =>
        await PostAsync(
            requestUrl,
            content: GetJsonContent(obj),
            token: token
        );

    protected virtual async Task<Result<TResult>> PostAsync<T, TResult>(string requestUrl, T obj, CancellationToken token = default) =>
        await PostAsync<TResult>(
            requestUrl,
            content: GetJsonContent(obj),
            token: token
        );

    protected virtual async Task<Result> PostAsync(string requestUrl, HttpContent content, CancellationToken token = default)
    {
        HttpRequestMessage message = CreateRequestMessage(HttpMethod.Post, CreateUri(requestUrl));
        message.Content = content;
        return await SendAsync(message, token: token);
    }

    protected virtual async Task<Result<TResult>> PostAsync<TResult>(string requestUrl, HttpContent content, CancellationToken token = default)
    {
        HttpRequestMessage message = CreateRequestMessage(HttpMethod.Post, CreateUri(requestUrl));
        message.Content = content;
        return await SendAsync<TResult>(message, token: token);
    }

    protected virtual async Task<Result> PatchAsync<T>(string requestUrl, T obj, CancellationToken token = default)
    {
        HttpRequestMessage message = CreateRequestMessage(HttpMethod.Patch, CreateUri(requestUrl));
        message.Content = GetJsonContent(obj);
        return await SendAsync(message, token: token);
    }

    protected virtual async Task<Result<Stream>> PostStreamAsync<T>(string requestUrl, T obj, CancellationToken token = default) =>
        await PostStreamAsync(
            requestUrl,
            content: GetJsonContent(obj),
            token
        );

    protected virtual async Task<Result<Stream>> PostStreamAsync(string requestUrl, HttpContent content, CancellationToken token = default)
    {
        HttpRequestMessage request = CreateRequestMessage(HttpMethod.Post, CreateUri(requestUrl));
        request.Content = content;

        Result<HttpResponseMessage> response = await SendRawAsync(
                request,
                HttpCompletionOption.ResponseHeadersRead,
                token
            );
        if (response)
            return Result<Stream>.Parse(
                    "",
                    await response.Body!.Content.ReadAsStreamAsync(token == default ? _cts.Token : token),
                    response.StatusCode
                );
        else
            return Result<Stream>.From(response);

    }

    protected virtual async Task<Result> SendAsync(HttpRequestMessage message, HttpCompletionOption option = HttpCompletionOption.ResponseContentRead, CancellationToken token = default)
    {
        Result<HttpResponseMessage> response = await SendRawAsync(message, option, token);
        if (response)
            return await Parse(response.Body!, token);
        else
            return Result.From(response);
    }

    protected virtual async Task<Result<TResult>> SendAsync<TResult>(HttpRequestMessage message, HttpCompletionOption option = HttpCompletionOption.ResponseContentRead, CancellationToken token = default)
    {
        Result<HttpResponseMessage> response = await SendRawAsync(message, option, token);
        if (response)
            return await Parse<TResult>(response.Body!, token);
        else
            return Result<TResult>.From(response);
    }

    protected virtual async Task<Result<HttpResponseMessage>> SendRawAsync(HttpRequestMessage message, HttpCompletionOption option = HttpCompletionOption.ResponseContentRead, CancellationToken token = default)
    {
        try
        {
            return Result<HttpResponseMessage>.OkResult(await _client.SendAsync(message, option, token == default ? _cts.Token : token));
        }
        catch (HttpRequestException)
        {
            return Result<HttpResponseMessage>.ErrorResult("There are network issues—either on your end or on ours", 200);
        }
        catch
        {
            return Result<HttpResponseMessage>.ErrorResult("Unexpected error", 211);
        }
    }


    protected async Task<Result<T>> Parse<T>(HttpResponseMessage response, CancellationToken token = default)
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

    protected async Task<Result> Parse(HttpResponseMessage response, CancellationToken token = default) =>
        Result.Parse(
            await response.Content.ReadAsStringAsync(token == default ? _cts.Token : token),
            (int)response.StatusCode
        );

    private HttpRequestMessage CreateRequestMessage(HttpMethod method, Uri? uri) =>
        new(method, uri) { Version = _defaultRequestVersion, VersionPolicy = _defaultVersionPolicy };

    private static Uri? CreateUri(string? uri) =>
        string.IsNullOrEmpty(uri) ? null : new Uri(uri, UriKind.RelativeOrAbsolute);

    protected StringContent GetJsonContent<T>(T obj) =>
        new(
            _serializer.Serialize(obj),
            Encoding.UTF8,
            "application/json"
        );

    public void Dispose()
    {
        _cts.Cancel();
        _client.Dispose();
        GC.SuppressFinalize(this);
    }
}