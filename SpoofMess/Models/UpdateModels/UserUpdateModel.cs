using CommonObjects.Requests.Changes;
using System.Collections.ObjectModel;

namespace SpoofMess.Models.UpdateModels;

public record class UserUpdateModel(
    Guid? Id = null,
    FileObject? Avatar = null,
    ObservableCollection<FileObject>? Avatars = null,
    string? Name = null,
    string? Login = null,
    DateTime? WasOnline = null
)
{
    public UserUpdateModel(User user) : this(user.Id, user.Avatar, user.Avatars, user.Name, user.Login, user.WasOnline)
    {

    }
}