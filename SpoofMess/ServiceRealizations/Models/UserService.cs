using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.Setters;
using System.Collections.Concurrent;
using System.IO;

namespace SpoofMess.ServiceRealizations.Models;

public class UserService(
    UserInfo userInfo, 
    IUserApiService userApiService, 
    IFileService fileService) : IUserService
{
    private readonly IUserApiService _userApiService = userApiService;
    private readonly IFileService _fileService = fileService;
    private readonly UserInfo _userInfo = userInfo;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    private ConcurrentDictionary<string, User> Users { get; set; } = [];

    public async Task<User?> Get(string login, byte[]? avatarId, byte[]? avatarToken, string originalAvatarName)
    {
        if(login == _userInfo.User.Login)
        {
            Users.TryAdd(login, _userInfo.User);
            return await UploadQueue(login, avatarId, avatarToken, originalAvatarName);
        }

        return await UploadQueue(login, avatarId, avatarToken, originalAvatarName);
    }

    private async Task<User?> UploadQueue(string login, byte[]? avatarId, byte[]? avatarToken, string originalAvatarName)
    {
        SemaphoreSlim semaphore = _locks.GetOrAdd(login, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync();
        try
        {
            if (Users.TryGetValue(login, out User? user))
            {
                await UploadAvatar(user, avatarId, avatarToken, originalAvatarName);
                return user;
            }

            Result<UserDTO> result = await _userApiService.GetByLogin(login);
            if (result.Success)
            {
                user = result.Body!.Set();
                Users.TryAdd(login, user);
                await UploadAvatar(user, avatarId, avatarToken, originalAvatarName);
                return user;
            }
            return null;
        }
        finally
        {
            semaphore.Release();
        }
    }

    private async Task UploadAvatar(
        User user, 
        byte[]? avatarId,
        byte[]? avatarToken,
        string originalAvatarName)
    {
        if(user.Avatar.Id != avatarId)
        {
            FileObject? oldAvatar = user.Avatars.FirstOrDefault(x => x.Id == avatarId);
            if (avatarId is not null && oldAvatar is null)
            {
                FileObject avatar = new()
                {
                    Id = avatarId,
                    Token = avatarToken,
                    Path = Path.Combine(_userInfo.SessionSettings.Directory, originalAvatarName)
                };
                user.Avatars.Add(avatar);
                user.Avatar = avatar;
                await _fileService.Save(avatar);
            }
            else if (oldAvatar is not null)
                user.Avatar = oldAvatar;
        }
    }

    public void OnUserUpdated(UpdateUserInfo updateUser)
    {
        if(Users.TryGetValue(updateUser.Login, out User? user))
        {
            user.Update(updateUser);
        }
    }
}
