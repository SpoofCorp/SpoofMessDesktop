using CommunityToolkit.Mvvm.ComponentModel;
using SpoofMess.Models;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class DesingViewModel(UserInfo userInfo,
                                     IThemesService themesService,
                                     IAuthService authService,
                                     IAppLifeCycleService appLifeCycleService) : AdditionalViewModel(appLifeCycleService)
{
    private readonly UserInfo _userInfo = userInfo;
    private readonly IThemesService _themesService = themesService;
    private readonly IAuthService _authService = authService;

    public static List<Theme> Themes { get; set; } =
    [
        new("Curb"),
        new("Dark"),
        new("Light"),
        new("MidNight"),
        new("Grayness"),
        new("TheColdAirOfRelief"),
        new("BlueFresh"),
        new("BerryMarshmallow"),
        new("DoubleLightning"),
        new("Milkshake")
    ];
    public static List<Visual> Visuals { get; set; } =
    [
        new("NewBase"),
        new("OldBase")
    ];

    [ObservableProperty]
    public partial Theme Theme { get; set; } = userInfo.Theme;

    [ObservableProperty]
    public partial Visual Visual { get; set; } = userInfo.Visual;

    public override void Initialize(IPlacementViewModel ownerView, Action close)
    {
        Theme = Themes.FirstOrDefault(x => x.Key == _userInfo.Theme.Key) ?? Themes.First();
        foreach (Theme theme in Themes)
            theme.Name = _themesService.GetLocaly(theme.Key);
        Visual = Visuals.FirstOrDefault(x => x.Key == _userInfo.Visual.Key) ?? Visuals.First();
        foreach (Visual visual in Visuals)
            visual.Name = _themesService.GetLocaly(visual.Key);
        base.Initialize(ownerView, close);
    }

    async partial void OnThemeChanged(Theme value)
    {
        _themesService.ChangeTheme(value);
        _userInfo.Theme = value;
        await _authService.Save(_appLifeCycleService.AppCancellationToken);
    }

    async partial void OnVisualChanged(Visual value)
    {
        _themesService.ChangeVisual(value);
        _userInfo.Visual = value;
        await _authService.Save(_appLifeCycleService.AppCancellationToken);
    }
}
