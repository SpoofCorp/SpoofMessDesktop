namespace SpoofMess.Models;

public enum Permissions : short
{
    //1xx: content and messages
    SendTexts = 100,
    SendAudios = 101,
    SendVideos = 102,
    SendFiles = 103,
    SendEmoji = 104,
    SendSticker = 105,
    SendVoiceMessage = 106,
    SendVideoMessage = 107,
    ShareMessage = 150,
    DeleteMessage = 151,
    EditMessage = 152,

    //2xx: moderation and roles
    CreateRole = 200,
    SetRole = 201,
    DepriveRole = 202,
    ChangeRules = 240,
    Muting = 260,
    Restricting = 261,

    //3xx: settings and members moderation
    Inviting = 300,
    Kicking = 301,
    Banning = 302,
    Sharing = 303,
    ChangeSettings = 330,
    ChangeAvatar = 331,
    DeleteAvatar = 332,
}