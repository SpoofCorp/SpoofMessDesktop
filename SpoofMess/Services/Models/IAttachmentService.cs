using CommonObjects.Requests.Attachments;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.ViewModels.FileViewModels;

namespace SpoofMess.Services.Models;

public interface IAttachmentService
{
    public Result Attach(
        MessageModel message);
    public void Unattach(
        FileObject file,
        MessageModel message);

    public Task UploadAttachments(MessageModel message, List<Attachment> attachments);
    public Task<Result<byte[]>> SendAttachment(FileObject file);
    public FileViewModel GetViewModel(FileObject file);
}
