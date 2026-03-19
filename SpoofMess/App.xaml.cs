using AdditionalHelpers.ServiceRealizations;
using AdditionalHelpers.Services;
using Microsoft.Extensions.DependencyInjection;
using SpoofFileInfo;
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
    protected override void OnStartup(StartupEventArgs e)
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
        services.AddSingleton<IFileClassifier, FileClassifier>();
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
        INavigationService? navigationService = _serviceProvider.GetService<INavigationService>();
        navigationService!.ShowEntryView();
    }

    protected override void OnExit(ExitEventArgs e)
    {
        base.OnExit(e);
    }
}
