using CommonObjects.DTO;
using CommonObjects.Results;
using SpoofMess.Models;
using SpoofMess.Services.Api;
using SpoofMess.Services.Models;
using System.Collections.Concurrent;

namespace SpoofMess.ServiceRealizations.Models;

public class ChatUserService(
    IChatUserApiService chatUserApiService,
    IChatService chatService,
    IUserService userService) : IChatUserService
{
    private readonly IChatUserApiService _chatUserApiService = chatUserApiService;
    private readonly IChatService _chatService = chatService;
    private readonly IUserService _userService = userService;

    public async Task SyncChats(DateTime after)
    {
        Result<List<ChatUserDTO>> chats = await _chatUserApiService.GetChats(after);
        if (chats.Success)
            await _chatService.AddChats(chats.Body!);
    }

    public async Task<bool> Join(Chat chat)
    {
        Result result = await _chatUserApiService.Join(new(chat.Id));
        return result.Success;
    }

    public async void ChatCreated(ChatUserDTO chatUserDTO)
    {
        await _chatService.AddChats([chatUserDTO]);
    }

    public async Task UploadManyMembers(Chat chat, CancellationToken cancellationToken = default)
    {
        ChatUser? member = chat.Users.OrderByDescending(x => x.User.WasOnline).FirstOrDefault() ?? new() { User = new() { WasOnline = null } };
        Result<List<ChatMemberDTO>> members = await _chatUserApiService.GetMembersBefore(chat.Id, member.User.Login, member.User.WasOnline, cancellationToken);
        if (!members)
            return;
        await AddUsers(chat, members.Body!, cancellationToken);
    }

    protected async Task AddUsers(Chat chat, List<ChatMemberDTO> members, CancellationToken cancellationToken = default)
    {
        ConcurrentBag<User> users = [];
        await Parallel.ForEachAsync(members, cancellationToken, async (member, ct) =>
        {
            User? user = await _userService.Get(member.Login, cancellationToken: cancellationToken);
            user ??= new()
            {
                Avatar = new()
                {
                    AttachmentToken = member.AvatarAccessToken,
                    Token = member.AvatarFileToken,
                    Name = member.OriginalFileName
                },
                Login = member.Login,
                Name = member.Name,
                WasOnline = member.WasOnline
            };
            users.Add(user);
        });
        lock (chat.Users)
            foreach (User user in users)
                if (!chat.Users.Any(x => x.User.Login == user.Login))
                    chat.Users.Add(new() { Chat = chat, User = user });
    }
}