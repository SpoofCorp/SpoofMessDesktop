using Microsoft.Extensions.DependencyInjection;
using SpoofMess.Services;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations;

public class AuthHandler(IServiceProvider serviceProvider) : DelegatingHandler
{
    private readonly IServiceProvider _serviceProvider = serviceProvider;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken
        )
    {
        string? access = await _serviceProvider.GetRequiredService<IAuthService>().GetAccess();
        if (!string.IsNullOrEmpty(access))
            request.Headers.Authorization = new("Bearer", access);

        return await base.SendAsync(request, cancellationToken);
    }
}
