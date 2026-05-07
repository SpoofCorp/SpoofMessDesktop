using CommonObjects.DTO;
using CommonObjects.Requests.Attachments;
using CommonObjects.Requests.Messages;
using SpoofMess.Models;
using SpoofMess.ViewModels.FileViewModels;
using System.Collections.ObjectModel;

namespace SpoofMess.Setters;

public static class MessageSetter
{
    public static MessageModel Set(this MessageDTO message) =>
        new()
        {
            Id = message.Id,
            ChatId = message.ChatId,
            SentAt = message.SendAt,
            Text = message.Text,
            User = new()
            {
                Login = message.SenderLogin,
                Name = message.SenderName,
            }
        };
    public static CreateMessageRequest Set(this MessageModel message, List<Attachment> attachments) =>
        new()
        {
            ChatId = message.ChatId,
            Text = message.Text ?? "",
            Attachments = attachments,
        };
    public static EditMessageRequest SetEdit(this EditMessageModel message, List<CommonObjects.Responses.EditAttachment> newAttachments) =>
        new()
        {
            Id = message.Id,
            ChatId = message.ChatId,
            Text = message.OldText == message.Text ? null : (message.Text ?? string.Empty),
            Attachments = newAttachments,
        };

    public static EditMessageModel GetEdit(this MessageModel message) =>
        new()
        {
            Id = message.Id,
            ChatId = message.ChatId,
            Text = message.Text ?? "",
            OldText = message.Text ?? "",
            Attachments = [.. message.Attachments.GetNewObjectViewModels()],
        };

    private static ObservableCollection<ObjectViewModel> GetNewObjectViewModels(this ObservableCollection<ObjectViewModel> objectViewModels)
    {
        ObservableCollection<ObjectViewModel> result = [];
        foreach (ObjectViewModel objectViewModel in objectViewModels)
        {
            result.Add(objectViewModel.Clone());
        }
        return result;
    }
}
