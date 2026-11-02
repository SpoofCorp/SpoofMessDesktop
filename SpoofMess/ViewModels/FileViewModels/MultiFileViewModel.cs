using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.ServiceRealizations;
using SpoofMess.Services;
using SpoofMess.ViewModels.Notifications;

namespace SpoofMess.ViewModels.FileViewModels;

public class MultiFileViewModel(
    IAppLifeCycleService appLifeCycleService,
    MusicNotification musicNotification) : BaseViewModel(appLifeCycleService)
{
    private readonly MusicNotification _musicNotification = musicNotification;
    public void OnFileInteract(FileObject currentFile, Chat currentChat)
    {
        if (currentFile is PlayableFileObject playableFileObject)
        {
            if(playableFileObject.Category == Enums.FileCategory.Audio || playableFileObject.Category == Enums.FileCategory.VoiceMessage)
            {
                _musicNotification.SongsList.Clear();
                _musicNotification.PlayResume(playableFileObject);
                _musicNotification.CurrentFile = (PlayableFileObject)currentFile;
                int index = 0;
                foreach (MusicViewModel musicVM in currentChat.Messages.SelectMany(x => x.Attachments).OfType<MusicViewModel>())
                {
                    foreach (PlayableFileObject file in musicVM.Files.Cast<PlayableFileObject>())
                    {
                        if(file == playableFileObject)
                        {
                            index++;
                            _musicNotification.AddSongWithIndex(playableFileObject);
                            continue;
                        }
                        _musicNotification.AddSong(file);
                        index++;
                    }
                }
                return;
            }
        }
    }
}
