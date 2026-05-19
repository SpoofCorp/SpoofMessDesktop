using CommonObjects.DTO;
using CommonObjects.Responses;
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
    IFileService fileService,
    IUserAvatarApiService userAvatarApiService,
    IAuthService authService) : IUserService
{
    private readonly IUserApiService _userApiService = userApiService;
    private readonly IFileService _fileService = fileService;
    private readonly IUserAvatarApiService _userAvatarApiService = userAvatarApiService;
    private readonly IAuthService _authService = authService;
    private readonly UserInfo _userInfo = userInfo;
    private static readonly ConcurrentDictionary<string, SemaphoreSlim> _locks = new();

    private ConcurrentDictionary<string, User> Users { get; set; } = [];

    public async Task<User?> Get(string login, CancellationToken cancellationToken = default)
    {
        if(login == _userInfo.User.Login)
            Users.TryAdd(login, _userInfo.User);

        return await UploadQueue(login, cancellationToken);
    }

    private async Task<User?> UploadQueue(string login, CancellationToken cancellationToken = default)
    {
        SemaphoreSlim semaphore = _locks.GetOrAdd(login, _ => new SemaphoreSlim(1, 1));
        await semaphore.WaitAsync(cancellationToken);
        try
        {
            if (Users.TryGetValue(login, out User? user))
            {
                return user;
            }

            Result<UserDTO> result = await _userApiService.GetByLogin(login);
            if (result.Success)
            {
                user = result.Body!.Set();
                Users.TryAdd(login, user);
                await UploadAvatar(user, result.Body!, cancellationToken);
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
        UserDTO userDTO,
        CancellationToken cancellationToken = default)
    {
        if(user.Avatar.Id != userDTO.AvatarId)
        {
            FileObject? oldAvatar = user.Avatars.FirstOrDefault(x => x.Id == userDTO.AvatarId);
            if (userDTO.AvatarId is not null && oldAvatar is null)
            {
                FileObject avatar = new()
                {
                    Id = userDTO.AvatarId,
                    Token = userDTO.AvatarToken,
                    Path = Path.Combine(_userInfo.SessionSettings.Directory, userDTO.AvatarOriginalFileName!)
                };
                user.Avatars.Add(avatar);
                user.Avatar = avatar;
                await _fileService.Save(avatar, cancellationToken);
            }
            else if (oldAvatar is not null)
                user.Avatar = oldAvatar;
        }
    }

    public async Task SyncAccount(CancellationToken cancellationToken = default)
    {
        User? user = await Get(_userInfo.User.Login, cancellationToken);
        if (user is null)
            return;
        _userInfo.User.Update(user);
    }

    public async void AvatarUpdateHandler(FileObject avatar, User user)
    {
        Result<AvatarResponse> result = await _userAvatarApiService.Get(avatar.AttachmentToken!);
        if (!result.Success)
            return;
        avatar.Path = Path.Combine(_userInfo.SessionSettings.Directory, avatar.Path ?? "");
        avatar.Token = result.Body!.FileMetadata.Token;
        avatar.Id = result.Body.FileMetadata.Id;
        user.Avatar = avatar;
        user.Avatars.Add(avatar);
        await _fileService.Save(avatar);
        if(_userInfo.User.Login == user.Login)
            await _authService.Save();
    }

    public void OnUserUpdated(UpdateUserInfo updateUser)
    {
        if(Users.TryGetValue(updateUser.Login, out User? user))
        {
            user.Update(updateUser);
        }
    }
}
