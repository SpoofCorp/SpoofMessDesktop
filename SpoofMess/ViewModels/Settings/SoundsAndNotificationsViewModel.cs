using CommonObjects.Requests.Changes;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using SpoofMess.Models;
using SpoofMess.Models.UpdateModels;
using SpoofMess.ServiceRealizations;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class SoundsAndNotificationsViewModel(
    AppSessionSettings appSessionSettings,
    IAuthService authService,
    IAppLifeCycleService appLifeCycleService,
    FileAudioRecorderService audioRecordService)
    : AdditionalViewModel(appLifeCycleService)
{
    private readonly FileAudioRecorderService _audioRecordService = audioRecordService;
    private readonly IAuthService _authService = authService;
    private readonly AppSessionSettings _appSessionSettings = appSessionSettings;
    public AppSessionSettings Edit { get; set; } = null!;
    public List<AudioInDevice> AudioInDevices { get; set; } = [];
    [ObservableProperty]
    public partial AudioInDevice? AudioInDevice { get; set; }

    protected override Task InitializeAsync(bool initialize)
    {
        Edit = new();
        Edit.Init(_appSessionSettings);
        using var enumerator = new MMDeviceEnumerator();
        var captureDevices = enumerator.EnumerateAudioEndPoints(DataFlow.Capture, DeviceState.Active);
        for (int i = 0; i < captureDevices.Count; i++)
            AudioInDevices.Add(
                new()
                { 
                    Name = captureDevices[i].FriendlyName,
                    Number = i
                }
            );
        AudioInDevice = _appSessionSettings.AudioInDevice is null ? AudioInDevices.FirstOrDefault() : AudioInDevices.FirstOrDefault(x => x.Name == _appSessionSettings.AudioInDevice.Name && x.Number == _appSessionSettings.AudioInDevice.Number);

        return base.InitializeAsync(initialize);
    }

    public override async void OnClose()
    {
        _appSessionSettings.Update(
            new UserInfoUpdateModel(
                ShowErrorNotifications: Edit.ShowErrorNotifications,
                ShowMessagesNotifications: Edit.ShowMessagesNotifications
            )
        );
        base.OnClose();
    }

    async partial void OnAudioInDeviceChanged(AudioInDevice? value)
    {
        _audioRecordService.ChangeDevice(value);
        _appSessionSettings.Update(new(AudioInDevice: value));
        await _authService.Save(_appLifeCycleService.AppCancellationToken);
    }
}
