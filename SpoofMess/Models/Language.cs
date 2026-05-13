namespace SpoofMess.Models;

public class Language(string name, string key, string cultureKey)
{
    public Language() : this(string.Empty, string.Empty, string.Empty)
    {

    }
    public string CultureKey { get; set; } = cultureKey;
    public string Key { get; set; } = key;
    public string Name { get; set; } = name;
}
