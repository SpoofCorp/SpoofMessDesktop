using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class LanguageViewModel(UserInfo userInfo, IAuthService authService, IThemesService themesService) : AdditionalViewModel
{
    private readonly UserInfo _userInfo = userInfo;
    private readonly IAuthService _authService = authService;
    private readonly IThemesService _themesService = themesService;
    public static List<Language> Languages { get; set; } = [new("Русский", "Ru", "ru-RU"), new("English", "En", "en-US"), new("Қазақша", "Kz", "kk-KZ")];
    [ObservableProperty]
    public partial Language Language { get; set; } = userInfo.Language;

    public override void Initialize(IPlacementViewModel ownerView, Action close)
    {
        Language = Languages.FirstOrDefault(x => x.Key == _userInfo.Language.Key) ?? Languages.First();
        base.Initialize(ownerView, close);
    }

    async partial void OnLanguageChanged(Language value)
    {
        _themesService.ChangeLanguage(value);
        _userInfo.Language = value;
        await _authService.Save();
    }
}