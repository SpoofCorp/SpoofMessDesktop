using AdditionalHelpers.ServiceRealizations;
using AdditionalHelpers.Services;
using Microsoft.Extensions.DependencyInjection;
using SpoofFileParser;
using SpoofFileParser.FileMetadataParser;
using SpoofFileParser.FileMetadataParser.Audio;
using SpoofMess.Models;
using SpoofMess.ServiceRealizations;
using SpoofMess.ServiceRealizations.Api;
using SpoofMess.ServiceRealizations.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.ViewModels;
using SpoofMess.ViewModels.FileViewModels;
using SpoofMess.ViewModels.Notifications;
using SpoofMess.ViewModels.Settings;
using SpoofMess.Views;
using System.IO;
using System.Windows;
using SpoofMess.Models.HttpClientExtensions;

namespace SpoofMess;

public partial class App : Application
{
    private IAppLifeCycleService _appLifiCycleServiceuthService = null!;
    private IServiceProvider _serviceProvider = null!;
    internal static UserInfo UserInfo = null!;
    protected override async void OnStartup(StartupEventArgs e)
    {
        FrameworkElement.LanguageProperty.OverrideMetadata(
            typeof(FrameworkElement),
            new FrameworkPropertyMetadata(System.Windows.Markup.XmlLanguage.GetLanguage(System.Globalization.CultureInfo.CurrentCulture.IetfLanguageTag))
        );

        ServiceCollection services = new();

        services.AddTransient<AuthHandler>();
        services.AddHttpClient<IEntryApiService, EntryApiService>(client =>
        {
            client.BaseAddress = new("https://localhost:443/ses/");
        });
        services.AddSSSCustomApi<IChatAvatarApiService, ChatAvatarApiService>();
        services.AddSSSCustomApi<IChatApiService, ChatApiService>();
        services.AddSSSCustomApi<IChatUserApiService, ChatUserApiService>();
        services.AddSSSCustomApi<IUserApiService, UserApiService>();
        services.AddSMSCustomApi<IAttachmentApiService, AttachmentApiService>();
        services.AddSMSCustomApi<IMessageApiService, MessageApiService>();

        services.AddSFSCustomApi<IFileApiService, FileApiService>();
        services.AddSSSCustomApi<IUserAvatarApiService, UserAvatarApiService>();
        services.AddSMSCustomApi<ISearchApiService, SearchApiService>();

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISerializer, JsonSerializerService>();
        services.AddSingleton<IDownloadService, DownloadService>();
        services.AddSingleton<FileAudioRecorderService>();

        /*services.AddSingleton(new UrlSettings()
        {
            BaseUrlSES = "https://localhost:5000",
            BaseUrlSFS = "https://localhost:5003",
            BaseUrlSMS = "https://localhost:5002",
            BaseUrlSSS = "https://localhost:5001",
        });*/

        /*services.AddSingleton(new UrlSettings()
        {
            BaseUrlSES = "https://192.168.0.54:5000",
            BaseUrlSFS = "https://192.168.0.54:5003",
            BaseUrlSMS = "https://192.168.0.54:5002",
            BaseUrlSSS = "https://192.168.0.54:5001",
        });*/

        services.AddTransient<EntryViewModel>();
        services.AddTransient<ChatCardViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<AdvancedViewModel>();
        services.AddTransient<SoundsAndNotificationsViewModel>();
        services.AddTransient<CreateGroupViewModel>();
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<ChatMemberCardViewModel>();
        services.AddSingleton<StaticNotificationViewModel>();
        ParserFactory factory = new(
            new()
            {
                [FileType.Image] =
                    new ImageMetadataParser(new()
                    {
                        ["jpeg"] = new(0, 2, 2, 2, true, [[0xFF, 0xC0], [0xFF, 0xC1], [0xFF, 0xC2]], 5),
                        ["jpg"] = new(0, 2, 2, 2, true, [[0xFF, 0xC0], [0xFF, 0xC1], [0xFF, 0xC2]], 5),
                        ["png"] = new(16, 4, 20, 4, true),
                        ["bmp"] = new(18, 4, 22, 4, false),
                        ["gif"] = new(6, 2, 8, 2, false),
                    }),
                [FileType.Audio] = new AudioMetadataParser(new()
                {
                    ["mp3"] = new Mp3MetadataParser()
                })
            },
            new FileMetadataParser());
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IFingerprintService, FingerprintService>();
        services.AddSingleton<IAttachmentService, AttachmentService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IUserAvatarService, UserAvatarService>();
        services.AddSingleton<IChatService, ChatService>();
        services.AddSingleton<IChatAvatarService, ChatAvatarService>();
        services.AddSingleton<IChatUserService, ChatUserService>();
        services.AddSingleton<IUserService, UserService>();
        services.AddSingleton<ISearchService, SearchService>();
        services.AddSingleton<MusicNotification>();

        services.AddSingleton<INotificationApiService, NotificationApiService>();

        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddTransient<IThemesService, ThemesService>();

        services.AddHttpClient<IAuthService, AuthService>(client =>
        {
            client.BaseAddress = new("https://localhost:443/ses/");
        }).AddAsKeyed(ServiceLifetime.Singleton);
        services.AddSingleton<IAppLifeCycleService, AppLifeCycleService>();

        services.AddSingleton<UserInfo>();

        services.AddTransient<MainViewModel>();
        services.AddTransient<LanguageViewModel>();
        services.AddTransient<DesingViewModel>();
        services.AddTransient<AuthorizationViewModel>();
        services.AddTransient<ChatInfoViewModel>();
        services.AddTransient<ProfileViewModel>();
        services.AddTransient<RegistrationViewModel>();
        services.AddTransient<ImageViewModel>();
        services.AddTransient<FileViewModel>();
        services.AddTransient<MusicViewModel>();
        services.AddSingleton<CentralViewModel>();
        services.AddSingleton<ChatsViewModel>();
        services.AddTransient<MessagesViewModel>();
        services.AddTransient<MinimalViewModel>();
        services.AddSingleton<MainMessagesViewModel>();
        services.AddSingleton<NotificationInfoViewModel>();

        services.AddTransient<MainViewTest>();
        services.AddTransient<MinimalWindow>();
        services.AddSingleton<CentralView>();
        IServiceProvider tempProvider = services.BuildServiceProvider();

        ISerializer serializer = tempProvider.GetRequiredService<ISerializer>();
        services.AddSingleton<IFileClassifier>(new FileClassifier(factory, await serializer.Deserialize<ExtensionRoadMap[]>(File.OpenRead("Startup\\FileExtensions.json"))));
        _serviceProvider = services.BuildServiceProvider();

        _appLifiCycleServiceuthService = _serviceProvider.GetRequiredService<IAppLifeCycleService>();
        await _appLifiCycleServiceuthService.OnStartupAsync();
        UserInfo = _serviceProvider.GetRequiredService<UserInfo>();
        IThemesService themesServices = _serviceProvider.GetRequiredService<IThemesService>();
        themesServices.Initialize();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        await _appLifiCycleServiceuthService.OnExitAsync();
        base.OnExit(e);
        Current.Shutdown(0);
    }
}