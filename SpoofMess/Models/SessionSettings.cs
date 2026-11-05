using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace SpoofMess.Models;

public partial class SessionSettings : ObservableObject
{
    [ObservableProperty]
    public partial string Directory { get; set; } = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Downloads");
}
