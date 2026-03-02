using SpoofMess.Services;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations;

public class AuthHandler(IAuthService authService) : DelegatingHandler
{
    private readonly IAuthService _authService = authService;

    protected override async Task<HttpResponseMessage> SendAsync(
        HttpRequestMessage request, 
        CancellationToken cancellationToken
        )
    {
        string? access = await _authService.GetAccess();
        if (!string.IsNullOrEmpty(access))
            request.Headers.Authorization = new("Bearer", access);

        return await base.SendAsync(request, cancellationToken);
    }
}
