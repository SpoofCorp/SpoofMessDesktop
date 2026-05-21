using CommonObjects.DTO;
using SpoofMess.Models;
using System.Collections.ObjectModel;

namespace SpoofMess.Services.Models;

public interface ISearchService
{
    public Task SimpleSearch(ObservableCollection<Chat> searchableEntities, ObservableCollection<SearchableMessageWithChat> searchableMessageWithChats, string query);
}
