using Discord.Interactions;

namespace Aidoneus.Plugin.Music.Commands;

public class MusicCommands : InteractionModuleBase {

    [SlashCommand("play", "Queue a song")]
    public async Task Queue(string query) {
        await RespondAsync($"You asked for {query}", ephemeral: true);
    }
}