namespace SpoofMess.Models;

public class Theme(string key)
{
    public Theme() : this(string.Empty)
    {

    }

    public string Key { get; set; } = key;
    public string Name { get; set; } = null!;
}