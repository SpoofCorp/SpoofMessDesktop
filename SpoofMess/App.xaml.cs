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
using SpoofMess.Views;
using System.IO;
using System.Windows;

namespace SpoofMess;

public partial class App : Application
{
    private IServiceProvider _serviceProvider;


    protected override async void OnStartup(StartupEventArgs e)
    {
        base.OnStartup(e);

        ServiceCollection services = new();

        services.AddTransient<AuthHandler>();
        services.AddHttpClient<IEntryApiService, EntryApiService>();
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

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISerializer, JsonSerializerService>();
        services.AddSingleton<IDownloadService, DownloadService>();

        services.AddScoped<EntryViewModel>();
        services.AddScoped<SettingsViewModel>();
        services.AddScoped<CreateGroupViewModel>();
        services.AddScoped<ProfileViewModel>();
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
        services.AddSingleton<IChatUserService, ChatUserService>();

        services.AddSingleton<INotificationApiService, NotificationApiService>();

        services.AddSingleton<IAudioService, AudioService>();
        services.AddScoped<MainViewModel>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserService, UserService>();

        services.AddSingleton<IAuthService, AuthService>();

        services.AddSingleton<UserInfo>();

        services.AddTransient<AuthorizationViewModel>();
        services.AddTransient<RegistrationViewModel>();
        services.AddTransient<ImageViewModel>();
        services.AddTransient<FileViewModel>();
        services.AddTransient<MusicViewModel>();
        services.AddSingleton<CentralViewModel>();

        services.AddTransient<RegistrationView>();
        services.AddTransient<AuthorizationView>();
        services.AddTransient<MainView>();
        services.AddSingleton<CentralView>();
        IServiceProvider tempProvider = services.BuildServiceProvider();

        ISerializer serializer = tempProvider.GetRequiredService<ISerializer>();
        services.AddSingleton<IFileClassifier>(new FileClassifier(factory, await serializer.Deserialize<ExtensionRoadMap[]>(File.OpenRead("startup\\FileExtensions.json"))));

        _serviceProvider = services.BuildServiceProvider();

        IAuthService authService = _serviceProvider.GetRequiredService<IAuthService>();
        INavigationService navigationService = _serviceProvider.GetRequiredService<INavigationService>();
        if (await authService.Initialize())
            navigationService!.ShowCentralViewWithMain();
        else
            navigationService!.ShowCentralViewWithAuthorization();
    }

    protected override async void OnExit(ExitEventArgs e)
    {
        IAuthService authService = _serviceProvider.GetRequiredService<IAuthService>();

        await authService.Save();
        base.OnExit(e);
    }
}
