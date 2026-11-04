using CommonObjects.Requests.Changes;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;

namespace SpoofMess.ViewModels.Settings;

public partial class ChatSettingsViewModel(
    IChatService chatService,
    IAppLifeCycleService appLifeCycleService) : AdditionalViewModel(appLifeCycleService)
{
    public ChangeChatSettingsRequest ChatSettings { get; set; } = new();
    public Chat? SelectedChat
    {
        get;
        set
        {
            field = value;
            if (field is null) return;
            ChatSettings = new()
            {
                ChatName = field.Name,
                ChatTypeId = field.ChatTypeId,
                Id = field.Id,
                IsPublic = field.IsPublic,
                UniqueName = field.UniqueName,
                OwnerId = null
            };
            OnPropertyChanged();
            OnPropertyChanged(nameof(ChatSettings));
        }
    }
    private readonly IChatService _chatService = chatService;

    [RelayCommand]
    public async Task Save()
    {
        if(IsChanged())
        {
            if (await _chatService.ChangeSettings(ChatSettings, _appLifeCycleService.AppCancellationToken))
                Close();
        }
    }

    private bool IsChanged()
    {
        if (SelectedChat is null)
            return false;
        if (ChatSettings.ChatName != SelectedChat.Name)
            return true;
        if (ChatSettings.IsPublic != SelectedChat.IsPublic)
            return true;
        if (ChatSettings.UniqueName != SelectedChat.UniqueName)
            return true;
        return false;
    }
}