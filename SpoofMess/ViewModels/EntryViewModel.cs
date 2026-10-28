using SpoofMess.Services;

namespace SpoofMess.ViewModels;

internal partial class EntryViewModel(
    IAppLifeCycleService appLifeCycleService
    ) : PlacementViewModel(appLifeCycleService)
{
}
