using CommunityToolkit.Mvvm.Input;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Notifications;

public partial class MusicNotification(IAudioService audioService) : AdditionalViewModel
{
    public IAudioService AudioService { get; set; } = audioService;

    [RelayCommand]
    private void PlayResume()
    {
        AudioService.PauseResume();
    }
}
