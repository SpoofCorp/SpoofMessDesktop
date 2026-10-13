using CommonObjects.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace SpoofMess.Models;

public partial class User : ObservableObject
{
    public Guid Id { get; set; }
    [ObservableProperty]
    public partial FileObject? Avatar { get; set; }
    public ObservableCollection<FileObject> Avatars { get; set; } = [];
    [ObservableProperty]
    public partial string? Name { get; set; }
    [ObservableProperty]
    public partial string Login { get; set; } = string.Empty;

    [ObservableProperty]
    public partial DateTime? WasOnline { get; set; }

    public void Update(User user)
    {
        Id = user.Id;
        Avatar = user.Avatar;
        Avatars = user.Avatars;
        Name = user.Name;
        Login = user.Login;
        WasOnline = user.WasOnline;
    }
    public void Update(UpdateUserInfo userInfo)
    {
        if (userInfo.FileId is not null && !Avatars.Any(x => x.Id.SequenceEqual(userInfo.FileId)))
        {
            FileObject avatar = new()
            {
                Category = Enums.FileCategory.Image,
                Name = userInfo.OriginalFileName,
                Id = userInfo.FileId,
                Path = userInfo.OriginalFileName,
                AttachmentToken = userInfo.AccessToken
            };
            ServiceRealizations.EventHandler.NotifyUserAvatarUpdated(avatar, this);
        }
        Name = userInfo.Name ?? Name;
        Login = userInfo.Login ?? Login;
    }
}