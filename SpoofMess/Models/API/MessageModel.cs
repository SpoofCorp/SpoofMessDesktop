using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SpoofMess.ViewModels.FileViewModels;
using System.Collections.ObjectModel;
using System.Windows;

namespace SpoofMess.Models;

public partial class MessageModel : ObservableObject, IDisposable
{
    public Guid Id { get; set; }
    public Guid UserId { get; set; }
    public Guid ChatId { get; set; }

    [NotifyPropertyChangedFor(nameof(PreviewText))]
    [ObservableProperty]
    private string? _text;
    [ObservableProperty]
    private DateTime _sentAt;
    [ObservableProperty]
    private User? user;
    [ObservableProperty]
    private Chat? chat;
    public string PreviewText
    {
        get
        {
            if (!string.IsNullOrEmpty(Text))
                return Text;
            return string.Join(" ", Attachments.Select(x => x.Icon));
        }
    }
    public MessageModel()
    {
        Attachments.CollectionChanged += Attachments_CollectionChanged;
    }

    public ObservableCollection<ObjectViewModel> Attachments { get; set; } = [];

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

    private void Attachments_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e) =>
        OnPropertyChanged(nameof(PreviewText));

    public void Dispose()
    {
        Attachments.CollectionChanged -= Attachments_CollectionChanged;
        GC.SuppressFinalize(this);
    }
}