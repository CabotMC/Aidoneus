using System.Text.Json;
using System.Text.Json.Serialization;
using Aidoneus.API.Persistence;

namespace Aidoneus.Plugins;

public class FileBackedPluginStorage : IPluginPersistenceProvider
{
    private string _fileName;
    private Dictionary<string, JsonElement> _cache;
    
    public FileBackedPluginStorage(string fileName)
    {
        this._fileName = fileName;
        if (!File.Exists(fileName))
        {
            _cache = new Dictionary<string, JsonElement>();
        }
        else
        {
            _cache = JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(File.ReadAllText(fileName));
        }
        
    }

    public bool Store(string key, object? value)
    {
        _cache[key] = JsonSerializer.SerializeToElement(value);
        return true;
    }

    public T? Get<T>(string key)
    {
        if (!_cache.ContainsKey(key))
        {
            return default;
        }

        return _cache[key].Deserialize<T>();
    }

    public bool Exists(string key)
    {
        return _cache.ContainsKey(key);
    }

    public void Save()
    {
        File.WriteAllText(_fileName, JsonSerializer.Serialize(_cache));
    }
}