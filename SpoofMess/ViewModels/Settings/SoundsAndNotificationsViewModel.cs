using CommonObjects.Requests.Changes;
using CommunityToolkit.Mvvm.ComponentModel;
using NAudio.CoreAudioApi;
using SpoofMess.Models;
using SpoofMess.Models.UpdateModels;
using SpoofMess.ServiceRealizations;
using SpoofMess.Services;

namespace SpoofMess.ViewModels.Settings;

public partial class SoundsAndNotificationsViewModel(
    UserInfo userInfo,
    IAuthService authService,
    IAppLifeCycleService appLifeCycleService,
    FileAudioRecorderService audioRecordService)
    : AdditionalViewModel(appLifeCycleService)
{
    private readonly FileAudioRecorderService _audioRecordService = audioRecordService;
    private readonly IAuthService _authService = authService;
    private readonly UserInfo _actualUserInfo = userInfo;
    public UserInfo Edit { get; set; } = null!;
    public List<AudioInDevice> AudioInDevices { get; set; } = [];
    [ObservableProperty]
    public partial AudioInDevice? AudioInDevice { get; set; }

    protected override Task InitializeAsync(bool initialize)
    {
        Edit = new();
        Edit.Init(_actualUserInfo);
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
        AudioInDevice = _actualUserInfo.AudioInDevice is null ? AudioInDevices.FirstOrDefault() : AudioInDevices.FirstOrDefault(x => x.Name == _actualUserInfo.AudioInDevice.Name && x.Number == _actualUserInfo.AudioInDevice.Number);

        return base.InitializeAsync(initialize);
    }

    public override async void OnClose()
    {
        if (_actualUserInfo.Change(Edit, out ChangeUserSettingsRequest request))
        {
            await _authService.Save(_appLifeCycleService.AppCancellationToken);
            _actualUserInfo.Update(new UserInfoUpdateModel(
                    ForwardMessage: request.ForwardMessage,
                    InviteMe: request.InviteMe,
                    MonthsBeforeDelete: request.MonthsBeforeDelete,
                    UserUpdateModel: new(Name: request.Name),
                    SearchMe: request.SearchMe,
                    ShowMe: request.ShowMe
            ));
        }
        base.OnClose();
    }

    async partial void OnAudioInDeviceChanged(AudioInDevice? value)
    {
        _audioRecordService.ChangeDevice(value);
        _actualUserInfo.Update(new(AudioInDevice: value));
        await _authService.Save(_appLifeCycleService.AppCancellationToken);
    }
}
