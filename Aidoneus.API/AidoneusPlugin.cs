using Aidoneus.API.Persistence;

namespace Aidoneus.API;

[AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
public class AidoneusPlugin : Attribute
{
    public string? minVersion;
    public AidoneusPlugin(string? minVersion = null) => this.minVersion = minVersion;

    public Version AsVersion() {
        if (minVersion == null) return new Version(0, 0, 0);
        return new Version(minVersion);
    }
}
public abstract class AidoneusPluginEntry
{
#pragma warning disable CS8618 
    private IPluginPersistenceProvider _persistence;
#pragma warning restore CS8618 
    public abstract void Initialize();

    protected IPluginPersistenceProvider GetPersistence()
    {
        return _persistence;
    }
}