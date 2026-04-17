using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Enums;
using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class LanguageViewModel(UserInfo userInfo, IThemesServices themesService) : AdditionalViewModel
{
    private readonly UserInfo _userInfo = userInfo;
    private readonly IThemesServices _themesService = themesService;
    public List<Language> Languages { get; set; } = [Language.Ru, Language.En];
    [ObservableProperty]
    private Language _language = userInfo.Language;

    partial void OnLanguageChanged(Language value)
    {
        _userInfo.Language = value;
        _themesService.ChangeLanguage(value);
    }
}
