using System.Diagnostics;
using System.Reflection;
using Discord;
using Discord.Interactions;

namespace Aidoneus.Commands;

public class UtilCommands : InteractionModuleBase {

    [SlashCommand("version", "Get the bot's version information")]
    public async Task VersionAsync() {
        var assembly = Assembly.GetEntryAssembly();
        
        var version = assembly?.GetName().Version;
        if (assembly == null || version == null) {
            await RespondAsync("FATAL: Could not get version");
            return;
        }

        var embed = new EmbedBuilder()
            .WithTitle("Aidoneus")
            .WithDescription($"v{version.Major}.{version.Minor}.{version.Build}")
            .WithColor(Color.Purple)

            .Build();
        await RespondAsync(embeds: new[] { embed });
    }

    [SlashCommand("stats", "Get the bot's statistics")]
    public async Task StatsAsync() {
        var uptime = DateTime.Now - Process.GetCurrentProcess().StartTime;

        var embed = new EmbedBuilder()
            .WithTitle("Aidoneus")
            .WithDescription("Stats")
            .WithColor(Color.Purple)
            .WithFields(
                new EmbedFieldBuilder()
                    .WithName("Uptime")
                    .WithValue($"{uptime.Days} days, {uptime.Hours} hours, {uptime.Minutes} minutes, {uptime.Seconds} seconds"),
                new EmbedFieldBuilder()
                    .WithName("Heap Size")
                    .WithValue($"{GC.GetTotalMemory(false) / 1024 / 1024} MB")
            )
            .Build();
        await RespondAsync(embeds: new[] { embed });
    }
}