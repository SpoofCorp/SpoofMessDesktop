using AdditionalHelpers.ServiceRealizations;
using AdditionalHelpers.Services;
using Microsoft.Extensions.DependencyInjection;
using SpoofFileParser;
using SpoofFileParser.FileMetadataParser;
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
using System.Windows;

namespace SpoofMess;

public partial class App : Application
{
    private IServiceProvider? _serviceProvider;


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

        services.AddSingleton<INavigationService, NavigationService>();
        services.AddSingleton<ISerializer, JsonSerializerService>();

        services.AddScoped<EntryViewModel>();
        services.AddScoped<SettingsViewModel>();
        services.AddScoped<CreateGroupViewModel>();
        services.AddScoped<ProfileViewModel>();
        ParserFactory factory = new([
            new ImageMetadataParser(new() {
                    ["jpeg"] = new(0, 2, 2, 2, true, [[0xFF, 0xC0], [0xFF, 0xC1], [0xFF, 0xC2]], 5),
                    ["jpg"] = new(0, 2, 2, 2, true, [[0xFF, 0xC0], [0xFF, 0xC1], [0xFF, 0xC2]], 5),
                    ["png"] = new(16, 4, 20, 4, true),
                    ["bmp"] = new(18, 4, 22, 4, false),
                    ["gif"] = new(6, 2, 8, 2, false),
                })]);
        services.AddSingleton<IFileClassifier>(new FileClassifier(factory));
        services.AddSingleton<IFileService, FileService>();
        services.AddSingleton<IFingerprintService, FingerprintService>();
        services.AddSingleton<IAttachmentService, AttachmentService>();
        services.AddSingleton<IMessageService, MessageService>();
        services.AddSingleton<IChatService, ChatService>();
        services.AddSingleton<IChatUserService, ChatUserService>();
        services.AddSingleton<INotificationApiService, NotificationApiService>();
        services.AddSingleton<IAudioService, AudioService>();
        services.AddScoped<MainViewModel>();
        services.AddScoped<INotificationService, NotificationService>();
        services.AddScoped<IUserService, UserService>();
        services.AddTransient<ImageViewModel>();
        services.AddTransient<FileViewModel>();
        services.AddTransient<MusicViewModel>();

        services.AddSingleton<IAuthService, AuthService>();

        services.AddSingleton<UserInfo>();
        services.AddSingleton<AuthorizationView>();
        services.AddSingleton<AuthorizationViewModel>();
        services.AddSingleton<RegistrationViewModel>();
        services.AddSingleton<RegistrationView>();
        services.AddSingleton<EntryWindow>();
        services.AddSingleton<MainView>();

        _serviceProvider = services.BuildServiceProvider();
        IAuthService? authService = _serviceProvider.GetRequiredService<IAuthService>();
        INavigationService? navigationService = _serviceProvider.GetRequiredService<INavigationService>();

        if (await authService.Initialize())
            navigationService!.ShowMainView();
        else
            navigationService!.ShowEntryView();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }
}
