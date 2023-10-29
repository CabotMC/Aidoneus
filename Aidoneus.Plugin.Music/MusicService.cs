using Victoria.Node;
using Victoria.Node.EventArgs;
using Victoria.Player;

namespace Aidoneus.Plugin.Music;

public class MusicService {
    LavaNode _lavaNode;
    public MusicService(LavaNode lavaNode) {
        _lavaNode = lavaNode;
        _lavaNode.OnTrackEnd += OnTrackEnd;
    }
    
    public async Task OnTrackEnd(TrackEndEventArg<LavaPlayer<LavaTrack>, LavaTrack> arg) {
        var player = arg.Player;
        if (player.Vueue.Count == 0) {
            await _lavaNode.LeaveAsync(player.VoiceChannel);
        } else {
            var success = player.Vueue.TryDequeue(out var nextTrack);
            await player.PlayAsync(nextTrack);
        }
    }
}