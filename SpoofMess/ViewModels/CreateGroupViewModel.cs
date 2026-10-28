using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;

namespace SpoofMess.ViewModels;

public partial class CreateGroupViewModel(IChatService chatService, IAppLifeCycleService appLifeCycleService) : AdditionalViewModel(appLifeCycleService)
{
    private readonly IChatService _chatService = chatService;

    public Chat Chat { get; set; } = new();

    [RelayCommand]
    private async Task Create()
    {
        if (string.IsNullOrWhiteSpace(Chat.Name) || string.IsNullOrWhiteSpace(Chat.UniqueName))
            return;

        await _chatService.CreateChat(
            Chat,
            _appLifeCycleService.AppCancellationToken
        );
    }
}
