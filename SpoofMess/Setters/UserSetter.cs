using CommonObjects.DTO;
using SpoofMess.Models;

namespace SpoofMess.Setters;

public static class UserSetter
{
    public static User Set(this UserDTO userDTO) =>
        new()
        {
            Avatar = new()
            {
                Token = userDTO.AvatarToken,
            },
            Login = userDTO.Login,
            Name = userDTO.Name,
        };
}