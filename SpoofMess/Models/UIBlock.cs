using CommunityToolkit.Mvvm.ComponentModel;

namespace SpoofMess.Models;

public partial class UIBlock : ObservableObject
{
    [ObservableProperty]
    public partial int Column { get; set; }

    [ObservableProperty]
    public partial int Row { get; set; }

    [ObservableProperty]
    public partial int ColumnSpan { get; set; }

    [ObservableProperty]
    public partial int RowSpan { get; set; }

    [ObservableProperty]
    public partial object DataContext { get; set; }
}
