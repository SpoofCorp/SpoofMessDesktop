using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services.Models;

namespace SpoofMess.ViewModels;

public partial class CreateGroupViewModel(IChatService chatService) : ObservableObject
{
    private readonly IChatService _chatService = chatService;
    public Chat Chat { get; set; } = new();

    [RelayCommand]
    private async Task Create()
    {
        if (string.IsNullOrWhiteSpace(Chat.Name) || string.IsNullOrWhiteSpace(Chat.UniqueName))
            return;

        await _chatService.CreateChat(Chat);
    }
}
