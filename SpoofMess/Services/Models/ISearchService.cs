using SpoofMess.Models;
using System.Collections.ObjectModel;

namespace SpoofMess.Services.Models;

public interface ISearchService
{
    public Task SimpleSearch(
        ObservableCollection<ISearchableEntity> searchableEntities,
        string query,
        CancellationToken cancellationToken = default
    );
}
