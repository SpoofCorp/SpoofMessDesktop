namespace SpoofMess.Models;

public class Visual(string key)
{
    public Visual() : this(string.Empty)
    {

    }

    public string Key { get; set; } = key;
    public string Name { get; set; } = null!;
}