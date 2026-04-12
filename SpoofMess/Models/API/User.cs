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
}
