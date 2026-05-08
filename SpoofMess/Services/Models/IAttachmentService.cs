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
        FileObject fileViewModel,
        MessageModel message);

    public Task UploadAttachments(MessageModel message, List<Attachment> attachments);
    public Task UploadAttachments(MessageModel message, List<CommonObjects.Responses.EditAttachment> attachments);
    public ObjectViewModel GetViewModel(FileObject file);
    public Task<Result<List<Attachment>>> SendAttachments(MessageModel message, CancellationToken token = default);
    public Task<Result<List<CommonObjects.Responses.EditAttachment>>> SendAttachments(EditMessageModel message, CancellationToken token = default);
    public Task<Result<byte[]>> SendAttachment(FileObject file, CancellationToken token = default);
}
