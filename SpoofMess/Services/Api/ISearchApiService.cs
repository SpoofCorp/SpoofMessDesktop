using CommonObjects.DTO;
using CommonObjects.Results;

namespace SpoofMess.Services.Api;

public interface ISearchApiService
{
    public Task<Result<List<SearchableEntity>>> SimpleSearchChats(string query);
    public Task<Result<List<SearchableMessage>>> SimpleSearchMessages(string query);
}
