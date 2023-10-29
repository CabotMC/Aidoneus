using Aidoneus.API;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Victoria;
using Victoria.Node;

namespace Aidoneus.Plugin.Music;

[AidoneusPlugin(minVersion = "1.0.3")]
public class MusicPlugin : AidoneusPluginEntry
{
    LavaNode _node;
    MusicService _service;

    ILogger<MusicPlugin> _logger;

    public MusicPlugin(LavaNode node, ILogger<MusicPlugin> logger) {
        _node = node;
        _service = new MusicService(node);
        _logger = logger;
    }
    public void Initialize()
    {
        _logger.LogInformation("Music plugin initialized");
    }
}