using Aidoneus.API;

namespace Aidoneus.Plugin.Music;

[AidoneusPlugin(minVersion = "1.0.0")]
public class Plugin : AidoneusPluginEntry
{
    public void Initialize()
    {
        Console.WriteLine("Hello from music!");
    }
}