using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services.Models;
using SpoofMess.Services.Api;
using SpoofMess.Setters;
using System.Collections.Concurrent;

namespace SpoofMess.ServiceRealizations.Models;

public class UserService(IUserApiService userApiService) : IUserService
{
    private readonly IUserApiService _userApiService = userApiService;
    private ConcurrentDictionary<string, User> Users { get; set; } = [];

    public async Task<User?> Get(string login)
    {
        if(Users.TryGetValue(login, out User? user)) return user;
        Result<UserDTO> result = await _userApiService.GetByLogin(login);
        if (result.Success)
        {
            user = result.Body!.Set();
            Users.TryAdd(login, user);
            return user;
        }
        return null;
    }
}
