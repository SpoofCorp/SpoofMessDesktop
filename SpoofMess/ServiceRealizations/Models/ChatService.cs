using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using SpoofMess.Setters;
using System.Collections.ObjectModel;

namespace SpoofMess.ServiceRealizations.Models;

public class ChatService(IChatApiService chatApiService) : IChatService
{
    public ObservableCollection<Chat> Chats { get; set; } = [];

    private readonly IChatApiService _chatApiService = chatApiService;

    public async Task<Chat?> Get(Guid id)
    {
        Chat? chat = Chats.FirstOrDefault(c => c.Id == id);
        if (chat is not null)
            return chat;

        Result<ChatDTO> chatResult = await _chatApiService.GetChat(id);
        if (!chatResult.Success)
            return null;
        chat = chatResult.Body!.Set();
        Chats.Add(chat);
        return chat;
    }

    public void AddChats(List<ChatUserDTO> chats)
    {
        foreach (ChatUserDTO chat in chats)
            Chats.Add(chat.Set());
    }

    public async Task CreateChat(Chat chat)
    {
        Result<ChatDTO> result = await _chatApiService.Create(new() 
            { 
                ChatName = chat.Name ?? string.Empty,
                UniqueName = chat.UniqueName,
                ChatTypeId = chat.ChatTypeId,
                IsPublic = chat.IsPublic
            });
        if(result.Success)
            Chats.Add(result.Body!.Set());
    }
}
