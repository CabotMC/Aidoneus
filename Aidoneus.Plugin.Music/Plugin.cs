using Aidoneus.API;
using Microsoft.Extensions.DependencyInjection;
using Victoria;
using Victoria.Node;

namespace Aidoneus.Plugin.Music;

[AidoneusPlugin(minVersion = "1.0.3")]
public class Plugin : AidoneusPluginEntry
{
    LavaNode _node;
    MusicService _service;

    public Plugin(LavaNode node) {
        _node = node;
        _service = new MusicService(node);
    }
    public void Initialize()
    {
        Console.WriteLine("Hello from music!");
    }
}