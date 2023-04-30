namespace LegendsToolbox.Core.Interfaces;

public interface IOptions : IAsyncObject
{
    public IAsyncEnumerable<KeyValuePair<string, object>> GetAllPropertiesAsync();
    public string GetString(string property, string defaultValue = "");
    public int GetInt(string property, int defaultValue = 0);
    public bool GetBoolean(string property, bool defaultValue = false);
    public void Set(string property, string value);
    public void Set(string property, int value);
    public void Set(string property, bool value);
}