using CommunityToolkit.Mvvm.Input;
using SpoofMess.Models;
using SpoofMess.Services.Models;
using System.Windows;

namespace SpoofMess.ViewModels;


public partial class ChatCardViewModel(
    Chat chat,
    IChatService chatService,
    IChatUserService chatUserService,
    IMessageService messageService) : AdditionalViewModel
{
    private readonly IChatService _chatService = chatService;
    private readonly IChatUserService _chatUserService = chatUserService;
    private readonly IMessageService _messageService = messageService;

    public Chat Chat { get; set; } = chat;
    public Visibility JoinVisibility => Chat.IsPublic ? Visibility.Visible : Visibility.Collapsed;

    private bool CanExecuteJoin() => Chat.IsPublic;

    [RelayCommand(CanExecute = nameof(CanExecuteJoin))]
    private async Task Join()
    {
        bool result = await _chatUserService.Join(Chat);
        if (result && _ownerView is ChatsViewModel mainViewModel)
        {
            await _chatService.Get(Chat.Id);
            mainViewModel.SetChat(Chat);
            await _messageService.UploadMessagesAfterDate(Chat);
        }
    }

}