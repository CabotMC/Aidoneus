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

public interface AidoneusPluginEntry
{
    public void Initialize();
}