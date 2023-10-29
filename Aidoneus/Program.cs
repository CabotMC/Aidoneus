
using System.Reflection;
using Aidoneus.API;
using Discord;
using Discord.WebSocket;

namespace Aidoneus;
public class Program {
    static DiscordSocketClient _client;

    public static void Main(string[] args) {
        var version = Assembly.GetAssembly(typeof(Program)).GetName().Version;
        Console.WriteLine($"Aidoneus v{version.Major}.{version.Minor}.{version.Build}");
        var token = Environment.GetEnvironmentVariable("AIDONEUS_TOKEN");
        if (token == null) {
            Console.WriteLine("FATAL: AIDONEUS_TOKEN not set");
            return;
        }

    }

    public static async Task AsyncMain(string[] args, string token) {
        _client = new DiscordSocketClient(new DiscordSocketConfig {
            LogLevel = args.Contains("v") ? Discord.LogSeverity.Verbose : Discord.LogSeverity.Info
        });

        await _client.LoginAsync(TokenType.Bot, token);
        await _client.StartAsync();
        await Task.Delay(-1); 
    }
}
