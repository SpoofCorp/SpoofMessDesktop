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

    public async Task SimpleSearch(
        ObservableCollection<ISearchableEntity> searchableEntities,
        string query,
        CancellationToken cancellationToken = default)
    {

        Task<Result<List<SearchableEntity>>> taskResultChats = _searchApiService.SimpleSearchChats(query, cancellationToken);

        Task<Result<List<SearchableMessageWithChat>>> taskResultMessages = SimpleSearchMessages(query, cancellationToken);
        searchableEntities.Clear();

        await Task.WhenAll(taskResultChats, taskResultMessages);

        if (taskResultChats.Result.Success)
        {
            List<Chat> chats = [];
            await Parallel.ForEachAsync(taskResultChats.Result.Body!, cancellationToken, async (searchableEntity, cancaletionToken) =>
            {
                if(searchableEntity.Type == SearchType.User)
                {
                    User? user = await _userService.Get(searchableEntity.UniqueName, cancellationToken: cancaletionToken);
                    if (user is null)
                        return;
                    
                }
                else
                {
                    Chat? chat = await _chatService.Get(searchableEntity.Id, false, cancellationToken);
                    if (chat is null) return;

                    chats.Add(chat);
                }
            });
            foreach (Chat chat in chats)
                searchableEntities.Add(chat);
        }
        if (taskResultMessages.Result.Success)
            foreach (SearchableMessageWithChat message in taskResultMessages.Result.Body!)
                searchableEntities.Add(message);
    }

    private async Task<Result<List<SearchableMessageWithChat>>> SimpleSearchMessages(
        string query,
        CancellationToken cancellationToken = default)
    {
        Result<List<SearchableMessage>> resultMessages = await _searchApiService.SimpleSearchMessages(
            query,
            cancellationToken
        );
        if (!resultMessages.Success)
            return Result<List<SearchableMessageWithChat>>.From(resultMessages);

        List<SearchableMessageWithChat> result = [];
        foreach (SearchableMessage searchableMessage in resultMessages.Body!)
        {
            Chat? chat = await _chatService.Get(
                searchableMessage.ChatId,
                cancellationToken: cancellationToken
            );
            if (chat is null)
                continue;
            result.Add(new(searchableMessage.Text, searchableMessage.SentAt, chat)
            {
                Id = searchableMessage.Id
            });
        }
        return Result<List<SearchableMessageWithChat>>.OkResult(result);
    }
}
