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

    public DateTime SentAt
    {
        get;
        set
        {
            field = value.ToLocalTime();
            OnPropertyChanged(nameof(SentAt));
        }
    }

    [NotifyPropertyChangedFor(nameof(PreviewText))]
    [ObservableProperty]
    public partial string? Text { get; set; }

    [ObservableProperty]
    public partial User? User { get; set; }

    [ObservableProperty]
    public partial Chat? Chat { get; set; }

    public Visibility CanChange => User?.Login == App.UserInfo.User.Login ? Visibility.Visible : Visibility.Collapsed;

    public Visibility HasAttachments => Attachments.Count > 0 ? Visibility.Visible : Visibility.Collapsed;

    public string PreviewText
    {
        get
        {
            return $"{(Attachments.Count > 0 ? string.Join(" ", Attachments.Select(x => x.Icon)) : "")}{ Text ?? ""}";
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
        ServiceRealizations.EventHandler.NotifyDelete(this);
    }

    [RelayCommand]
    private void Edit()
    {
        ServiceRealizations.EventHandler.NotifyEdit(this);
    }

    [RelayCommand]
    private void Copy()
    {
        Clipboard.SetText(Text ?? "");
    }

    private void Attachments_CollectionChanged(object? sender, System.Collections.Specialized.NotifyCollectionChangedEventArgs e)
    {
        OnPropertyChanged(nameof(PreviewText));
        OnPropertyChanged(nameof(HasAttachments));
    }

    public void Dispose()
    {
        Attachments.CollectionChanged -= Attachments_CollectionChanged;
        GC.SuppressFinalize(this);
    }
}