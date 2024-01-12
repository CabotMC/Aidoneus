namespace Aidoneus.API.Persistence;

public interface IPluginPersistenceProvider
{
    bool Store(string key, object? value);
    
    T? Get<T>(string key);
    
    bool Exists(string key);
}