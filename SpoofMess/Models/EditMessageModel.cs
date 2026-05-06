namespace SpoofMess.Models;

public partial class EditMessageModel : MessageModel
{
    public string OldText { get; set; } = string.Empty;

    public List<EditAttachment> NewAttachments { get; set; } = [];
}