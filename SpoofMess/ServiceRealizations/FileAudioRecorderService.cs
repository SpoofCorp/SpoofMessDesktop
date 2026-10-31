using CommonObjects.Results;
using NAudio.Wave;
using SpoofMess.Models;
using SpoofMess.Services;
using System.IO;

namespace SpoofMess.ServiceRealizations;

public partial class FileAudioRecorderService(IFileService file, UserInfo userInfo) : AudioRecorderService(userInfo)
{
    private WaveFileWriter? tempWavWriter;
    private string? _currentFile;
    private readonly List<string> _files = [];
    private readonly IFileService _file = file;
    private TaskCompletionSource<string?>? TaskCompletionSource;

    public async Task<Result<FileObject>> RecordToFile()
    {
        await Record();
        return TaskCompletionSource is null
            ? Result<FileObject>.BadRequest("")
            : await TaskCompletionSource.Task is string path
                ? _file.GetFileInfo(path)
                : Result<FileObject>.ErrorResult("Record error");

    }

    public override async Task Record()
    {
        if (TaskCompletionSource is not null)
            await TaskCompletionSource.Task;
        TaskCompletionSource = new();
        _currentFile = $"{DateTime.UtcNow:HH-mm-ss-fffff dd-MM-yyyy}.wav";
        tempWavWriter ??= new(_currentFile, waveFormat);
        await base.Record();
    }

    protected override void OnRecordingStopped(object? sender, StoppedEventArgs e)
    {
        tempWavWriter?.Dispose();
        tempWavWriter = null;
        string path;
        using (var reader = new WaveFileReader(_currentFile))
        {
            path = $"{_currentFile?[..^3]}mp3";
            MediaFoundationEncoder.EncodeToMp3(reader, path, 192000);
            _files.Add(path);
        }

        if (File.Exists(_currentFile))
            File.Delete(_currentFile);

        base.OnRecordingStopped(sender, e);
        TaskCompletionSource?.SetResult(path);
    }

    protected override void WaveInDataAvailable(object? sender, WaveInEventArgs e)
    {
        tempWavWriter?.Write(e.Buffer, 0, e.BytesRecorded);
        base.WaveInDataAvailable(sender, e);
    }

    public string? GetLastAudio() =>
        _files.LastOrDefault();
}