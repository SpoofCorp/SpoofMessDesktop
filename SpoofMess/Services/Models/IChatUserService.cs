namespace SpoofMess.Services.Models;

public interface IChatUserService
{
    public Task SyncChats(DateTime after);
}
