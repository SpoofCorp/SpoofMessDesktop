using CommunityToolkit.Mvvm.Input;
using SpoofMess.Bases;
using SpoofMess.Models;
using SpoofMess.Services;
using SpoofMess.Services.Models;
using System.Windows;

namespace SpoofMess.ViewModels;


public partial class ChatCardViewModel(
    IChatService chatService,
    IChatUserService chatUserService,
    IMessageService messageService,
    IAppLifeCycleService appLifeCycleService) : AdditionalViewModel(appLifeCycleService)
{
    private readonly IChatService _chatService = chatService;
    private readonly IChatUserService _chatUserService = chatUserService;
    private readonly IMessageService _messageService = messageService;

    public Chat Chat { get; set; } = null!;
    public Visibility JoinVisibility => Chat.IsPublic ? Visibility.Visible : Visibility.Collapsed;

    private bool CanExecuteJoin() => Chat.IsPublic;

    [RelayCommand(CanExecute = nameof(CanExecuteJoin))]
    private async Task Join()
    {
        bool result = await _chatUserService.Join(
            Chat,
            _appLifeCycleService.AppCancellationToken
        );
        if (result && _placement is ChatsViewModel mainViewModel)
        {
            await _chatService.Get(
                Chat.Id,
                cancellationToken: _appLifeCycleService.AppCancellationToken
            );
            mainViewModel.SetChat(Chat);
            await _messageService.UploadMessagesAfterDate(
                Chat,
                _appLifeCycleService.AppCancellationToken
            );
        }
    }

}