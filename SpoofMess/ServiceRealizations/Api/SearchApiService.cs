using AdditionalHelpers.Services;
using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Services.Api;
using System.Net.Http;

namespace SpoofMess.ServiceRealizations.Api;

internal class SearchApiService(
    HttpClient client,
    ISerializer serializer) : ApiService(
        client,
        serializer), ISearchApiService
{
    protected override string BaseUrl => "https://localhost:7146/api/Search";

    public async Task<Result<List<SearchableEntity>>> SimpleSearchChats(string query) =>
        await GetAsync<List<SearchableEntity>>($"/simple-search-chats?query={query}");
    public async Task<Result<List<SearchableMessage>>> SimpleSearchMessages(string query) =>
        await GetAsync<List<SearchableMessage>>($"/simple-search-messages?query={query}");
}
