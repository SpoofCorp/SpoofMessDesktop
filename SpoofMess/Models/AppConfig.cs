using CommunityToolkit.Mvvm.ComponentModel;
using System.IO;

namespace SpoofMess.Models;

public partial class AppConfig : ObservableObject
{
    public string BaseUrl { get; set; } = "https://localhost:443/";
    [ObservableProperty]
    public partial string Directory { get; set; } = Path.Combine(System.IO.Directory.GetCurrentDirectory(), "Downloads");
}
