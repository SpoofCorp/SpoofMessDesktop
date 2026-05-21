using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Collections.ObjectModel;

namespace SpoofMess.ServiceRealizations.Models;

public class SearchService(
    ISearchApiService searchApiService,
    INotificationService notificationService,
    IChatService chatService,
    IUserService userService,
    UserInfo userInfo) : ISearchService
{
    private readonly ISearchApiService _searchApiService = searchApiService;
    private readonly INotificationService _notificationService = notificationService;
    private readonly IChatService _chatService = chatService;
    private readonly IUserService _userService = userService;
    private readonly UserInfo userInfo = userInfo;

    public async Task SimpleSearch(ObservableCollection<Chat> searchableEntities, ObservableCollection<SearchableMessageWithChat> searchableMessageWithChats, string query)
    {

        Task<Result<List<SearchableEntity>>> taskResultChats = _searchApiService.SimpleSearchChats(query);

        Task<Result<List<SearchableMessageWithChat>>> taskResultMessages = SimpleSearchMessages(query);
        searchableMessageWithChats.Clear();

        await Task.WhenAll(taskResultChats, taskResultMessages);
        if (taskResultMessages.Result.Success)
            searchableMessageWithChats.ClearAndAddRange(taskResultMessages.Result.Body!);

        if (taskResultChats.Result.Success)
        {
            List<Chat> chats = [];
            await Parallel.ForEachAsync(taskResultChats.Result.Body!, async (searchableEntity, cancaletionToken) =>
            {
                if(searchableEntity.Type == SearchType.User)
                {
                    User? user = await _userService.Get(searchableEntity.UniqueName, cancaletionToken);
                    if (user is null)
                        return;
                    
                }
                else
                {
                    Chat? chat = await _chatService.Get(searchableEntity.Id, false);
                    if (chat is null) return;

                    chats.Add(chat);
                }
            });
            searchableEntities.ClearAndAddRange(chats);
        }
    }

    private async Task<Result<List<SearchableMessageWithChat>>> SimpleSearchMessages(string query)
    {
        Result<List<SearchableMessage>> resultMessages = await _searchApiService.SimpleSearchMessages(query);
        if (!resultMessages.Success)
            return Result<List<SearchableMessageWithChat>>.From(resultMessages);

        List<SearchableMessageWithChat> result = [];
        foreach (SearchableMessage searchableMessage in resultMessages.Body!)
        {
            Chat? chat = await _chatService.Get(searchableMessage.ChatId);
            if (chat is null)
                continue;
            result.Add(new(searchableMessage.Id, searchableMessage.Text, searchableMessage.SentAt, chat));
        }
        return Result<List<SearchableMessageWithChat>>.OkResult(result);
    }
}
