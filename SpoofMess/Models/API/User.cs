using CommonObjects.DTO;
using CommunityToolkit.Mvvm.ComponentModel;
using System.Collections.ObjectModel;

namespace SpoofMess.Models;

public partial class User : ObservableObject
{
    public Guid Id { get; set; }
    [ObservableProperty]
    private FileObject _avatar = new()
    {
        
    };
    public ObservableCollection<FileObject> Avatars { get; set; } = [];
    [ObservableProperty]
    private string? _name;
    [ObservableProperty]
    private string _login = string.Empty;

    public void Update(User user)
    {
        Id = user.Id;
        Avatar = user.Avatar;
        Avatars = user.Avatars;
        Name = user.Name;
        Login = user.Login;
    }
    public void Update(UpdateUserInfo userInfo)
    {
        if(userInfo.FileId is not null)
        {
            Avatar = new()
            {
                Category = Enums.FileCategory.Image,
                Name = userInfo.OriginalFileName,
                Id = userInfo.FileId
            };
        }
        Name = userInfo.Name;
        Login = userInfo.Login;
    }
}
