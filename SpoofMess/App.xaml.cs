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

namespace SpoofMess;

public partial class App : Application
{
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
        services.AddHttpClient<IEntryApiService, EntryApiService>();
        services.AddHttpClient<IChatAvatarApiService, ChatAvatarApiService>()
            .AddHttpMessageHandler<AuthHandler>();
        services.AddHttpClient<IAttachmentApiService, AttachmentApiService>()
            .AddHttpMessageHandler<AuthHandler>();
        services.AddHttpClient<IChatApiService, ChatApiService>()
            .AddHttpMessageHandler<AuthHandler>();
        services.AddHttpClient<IChatUserApiService, ChatUserApiService>()
            .AddHttpMessageHandler<AuthHandler>();
        services.AddHttpClient<IUserApiService, UserApiService>()
            .AddHttpMessageHandler<AuthHandler>();
        services.AddHttpClient<IMessageApiService, MessageApiService>()
            .AddHttpMessageHandler<AuthHandler>();
        services.AddHttpClient<IFileApiService, FileApiService>()
            .AddHttpMessageHandler<AuthHandler>();
        services.AddHttpClient<IUserAvatarApiService, UserAvatarApiService>()
            .AddHttpMessageHandler<AuthHandler>();
        services.AddHttpClient<ISearchApiService, SearchApiService>()
            .AddHttpMessageHandler<AuthHandler>();

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISerializer, JsonSerializerService>();
        services.AddSingleton<IDownloadService, DownloadService>();

        services.AddTransient<EntryViewModel>();
        services.AddTransient<SettingsViewModel>();
        services.AddTransient<AdvancedViewModel>();
        services.AddTransient<SoundsAndNotificationsViewModel>();
        services.AddTransient<CreateGroupViewModel>();
        services.AddTransient<ProfileViewModel>();
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

        services.AddSingleton<INotificationApiService, NotificationApiService>();

        services.AddSingleton<IAudioService, AudioService>();
        services.AddSingleton<INotificationService, NotificationService>();
        services.AddTransient<IThemesService, ThemesService>();

        services.AddSingleton<IAuthService, AuthService>();

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
        services.AddSingleton<MessagesViewModel>();
        services.AddSingleton<NotificationInfoViewModel>();

        services.AddTransient<MainView>();
        services.AddSingleton<CentralView>();
        IServiceProvider tempProvider = services.BuildServiceProvider();
        
        ISerializer serializer = tempProvider.GetRequiredService<ISerializer>();
        services.AddSingleton<IFileClassifier>(new FileClassifier(factory, await serializer.Deserialize<ExtensionRoadMap[]>(File.OpenRead("startup\\FileExtensions.json"))));
        _serviceProvider = services.BuildServiceProvider();

        IAuthService authService = _serviceProvider.GetRequiredService<IAuthService>();
        StaticNotificationViewModel staticNotificationViewModel = _serviceProvider.GetRequiredService<StaticNotificationViewModel>();
        staticNotificationViewModel.ViewModel = new MusicNotification(_serviceProvider.GetRequiredService<IAudioService>());
        IUserService userService = _serviceProvider.GetRequiredService<IUserService>();
        INotificationService notificationService = _serviceProvider.GetRequiredService<INotificationService>();
        INavigationService navigationService = _serviceProvider.GetRequiredService<INavigationService>();
        notificationService.InitializeAsync();
        navigationService.InitializeAsync();
        if (await authService.Initialize())
        {
            await userService.SyncAccount();
            navigationService!.ShowCentralViewWithMain();
        }
        else
            navigationService!.ShowCentralViewWithAuthorization();
        UserInfo = _serviceProvider.GetRequiredService<UserInfo>();
        IThemesService themesServices = _serviceProvider.GetRequiredService<IThemesService>();
        themesServices.Initialize();
        base.OnStartup(e);
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
        Current.Shutdown(0);
    }
}
