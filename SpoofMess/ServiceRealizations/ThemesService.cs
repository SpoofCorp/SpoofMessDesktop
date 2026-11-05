using SpoofMess.Models;
using SpoofMess.Services;
using System.Globalization;
using System.Windows;

namespace SpoofMess.ServiceRealizations;

public class ThemesService(AppSessionSettings appSessionSettings) : IThemesService
{
    private readonly AppSessionSettings _appSessionSettings = appSessionSettings;

    public void ChangeLanguage(Language language)
    {
        if (language.Key == _appSessionSettings.Language.Key)
            return;
        ChangeLanguageLocal(language);
    }
    public void ChangeTheme(Theme theme)
    {
        if (theme.Key == _appSessionSettings.Theme.Key)
            return;
        ChangeThemeLocal(theme);
    }

    public string GetLocaly(string key)
    {
        if (Application.Current.TryFindResource(key) is string localizedString)
            return localizedString;
        return key;
    }

    public void Initialize()
    {
        ChangeLanguageLocal(_appSessionSettings.Language);
        ChangeThemeLocal(_appSessionSettings.Theme);
        ChangeVisualLocal(_appSessionSettings.Visual);
    }

    private void ChangeLanguageLocal(Language language)
    {
        Change(GetLanguagePath(language), 0);
        _appSessionSettings.CurrentCultureInfo = new(language.CultureKey);
        CultureInfo.CurrentCulture = _appSessionSettings.CurrentCultureInfo;
        CultureInfo.CurrentUICulture = _appSessionSettings.CurrentCultureInfo; 
        Thread.CurrentThread.CurrentCulture = _appSessionSettings.CurrentCultureInfo;
        Thread.CurrentThread.CurrentUICulture = _appSessionSettings.CurrentCultureInfo;

        Application.Current.MainWindow.Language = System.Windows.Markup.XmlLanguage.GetLanguage(_appSessionSettings.CurrentCultureInfo.IetfLanguageTag);
    }

    private static void ChangeThemeLocal(Theme theme) =>
        Change(GetThemePath(theme), 1);
    private static void ChangeVisualLocal(Visual visual) =>
        Change(GetVisualPath(visual), 2);

    private static void Change(string fileName, int index)
    {
        Application.Current.Resources.MergedDictionaries[index] = new() { Source = new(fileName, UriKind.Relative) };
    }

    private static string GetLanguagePath(Language language) =>
        $"Resources/Localizations/{language.Key}.xaml";
    private static string GetThemePath(Theme theme) =>
        $"Resources/Themes/{theme.Key}.xaml";
    private static string GetVisualPath(Visual visual) =>
        $"Resources/Visuals/{visual.Key}.xaml";

    public void ChangeVisual(Visual visual)
    {
        if (visual.Key == _appSessionSettings.Visual.Key)
            return;
        ChangeVisualLocal(visual);
    }
}