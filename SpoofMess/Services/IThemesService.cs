using SpoofMess.Models;

namespace SpoofMess.Services;

public interface IThemesServices
{
    public void ChangeLanguage(Language language);
    public void ChangeTheme(Theme theme);
    public void ChangeVisual(Visual visual);
    public void Initialize();
    public string GetLocaly(string key);
}
