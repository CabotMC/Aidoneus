using System.Text;
using Aidoneus.API.Preconditions;
using Discord;
using Discord.Interactions;
using Victoria.Node;
using Victoria.Player;
using Victoria.Responses.Search;

namespace Aidoneus.Plugin.Music.Commands;

public enum SearchOptions {
    YouTube = SearchType.YouTube,
    Soundcloud = SearchType.SoundCloud,
    [ChoiceDisplay("Youtube Music")]
    YoutubeMusic = SearchType.YouTubeMusic
}

public class MusicCommands : InteractionModuleBase {

    LavaNode _lavaNode;
    public MusicCommands(LavaNode lavaNode) {
        _lavaNode = lavaNode;
    }

    [RequireContext(ContextType.Guild)]
    [RequireVoiceConnection]
    [SlashCommand("play", "Queue a song")]
    public async Task Queue(string query, SearchOptions searchType = SearchOptions.Soundcloud) {
        var existingPlayer = _lavaNode.TryGetPlayer(Context.Guild, out var player);
        var guildUser = await Context.Guild.GetUserAsync(Context.User.Id);
        if (!existingPlayer) {
            player = await _lavaNode.JoinAsync(guildUser.VoiceChannel, Context.Channel as ITextChannel);
        }
        var search = await _lavaNode.SearchAsync((SearchType) searchType, query);
        if (search.Status == SearchStatus.LoadFailed || search.Status == SearchStatus.NoMatches) {
            await RespondAsync("Error searching for tracks", ephemeral: false);
            return;
        }

        var track = search.Tracks.FirstOrDefault();
        if (track == null) {
            await RespondAsync("No tracks found", ephemeral: false);
            return;
        }
        if (player.PlayerState == PlayerState.Playing || player.PlayerState == PlayerState.Paused) {
            player.Vueue.Enqueue(track);
            await RespondAsync($"Enqueued {track.Title}", ephemeral: true);
        } else {
            await player.PlayAsync(track);
            await RespondAsync($"Now Playing {track.Title}", ephemeral: false);
        }
    }

    [RequireContext(ContextType.Guild)]
    [SlashCommand("stop", "Stop the player")]
    public async Task Stop() {
        var existingPlayer = _lavaNode.TryGetPlayer(Context.Guild, out var player);
        if (!existingPlayer) {
            await RespondAsync("No player found", ephemeral: true);
            return;
        }

        await _lavaNode.LeaveAsync(player.VoiceChannel);
        await RespondAsync("Stopped player", ephemeral: true);
    }

    [RequireContext(ContextType.Guild), RequireVoiceConnection()]
    [SlashCommand("skip", "Skip the current track")]
    public async Task Skip() {
        var existingPlayer = _lavaNode.TryGetPlayer(Context.Guild, out var player);
        if (!existingPlayer) {
            await RespondAsync("No player found", ephemeral: true);
            return;
        }
        await player.StopAsync();
        await RespondAsync("Skipped track", ephemeral: true);
        
    }

    [RequireContext(ContextType.Guild), RequireVoiceConnection]
    [SlashCommand("queue", "Show the current queue")]
    public async Task Queue()
    {
        
        var existingPlayer = _lavaNode.TryGetPlayer(Context.Guild, out var player);
        if (!existingPlayer) {
            await RespondAsync("No player found", ephemeral: true);
            return;
        }

        var queue = player.Vueue.ToArray();
        var result = new StringBuilder("```\n");
        result.Append("Now Playing:\n " + player.Track.Title + "\n\n");
        for (var i = 0; i < queue.Length; i++) {
            result.Append($"{i + 1}. {queue[i].Title}\n");
        }
        result.Append("```");
        await RespondAsync(result.ToString());
    }
}