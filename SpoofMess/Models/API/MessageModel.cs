using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.ServiceRealizations;
using SpoofMess.ViewModels.FileViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpoofMess.Models;

public partial class MessageModel : ObservableObject
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }

    [ObservableProperty]
    private string? _text;
    [ObservableProperty]
    private DateTime _sentAt;
    [ObservableProperty]
    private User? user;
    [ObservableProperty]
    private Chat? chat;

    public ObservableCollection<FileViewModel> Attachments { get; set; } = [];

    [RelayCommand]
    private void Delete()
    {
        ServiceRealizations.EventHandler.OnDelete(this);
    }
    [RelayCommand]
    private void Edit()
    {
        ServiceRealizations.EventHandler.OnEdit(this);
    }
    [RelayCommand]
    private void Copy()
    {
        Clipboard.SetText(Text ?? "");
    }
}