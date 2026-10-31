using CommonObjects.Responses;
using System.Globalization;

namespace SpoofMess.Models.UpdateModels;

public record class UserInfoUpdateModel(
    string? Password = null,
    double? Width = null,
    double? Height = null,
    bool? HideToTray = null,
    bool? UnselectChat = null,
    bool? ShowMessagesNotifications = null,
    bool? ShowErrorNotifications = null,
    bool? SearchMe = null,
    bool? ShowMe = null,
    bool? ForwardMessage = null,
    bool? InviteMe = null,
    int? MonthsBeforeDelete = null,
    string? EditedName = null,
    Language? Language = null,
    Theme? Theme = null,
    Visual? Visual = null,
    bool? CloneWindows = null,
    CultureInfo? CurrentCultureInfo = null,
    AudioInDevice? AudioInDevice = null,
    UserAuthorizeResponse? Authorize = null,
    SessionSettings? SessionSettings = null,
    UserUpdateModel? UserUpdateModel = null
);