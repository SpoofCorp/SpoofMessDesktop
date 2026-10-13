using Microsoft.Extensions.DependencyInjection;
using SpoofMess.ServiceRealizations;

namespace SpoofMess.Models.HttpClientExtensions;

public static class SFSHttpClientExtensions
{
    private static string _baseUrl = "https://localhost:443/";
    public static IHttpClientBuilder AddSFSCustomApi<TInterface, TImplementation>(this IServiceCollection services)
        where TInterface : class
        where TImplementation : class, TInterface
    {
        return services.AddHttpClient<TInterface, TImplementation>(client =>
        {
            client.BaseAddress = new Uri($"{_baseUrl}sfs/");
        })
        .AddHttpMessageHandler<AuthHandler>();
    }

    public static void SetBaseUrl(string baseUrl) =>
        _baseUrl = baseUrl;
}