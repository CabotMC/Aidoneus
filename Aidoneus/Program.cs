
using System.Reflection;
using Aidoneus.Plugins;
using Discord;
using Discord.Interactions;
using Discord.WebSocket;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using Victoria;

namespace Aidoneus;

public class Program
{
    private readonly IServiceProvider _serviceProvider;

    public Program()
    {
        _serviceProvider = CreateProvider();
    }

    static void Main(string[] args)
        => new Program().RunAsync(args).GetAwaiter().GetResult();

    static IServiceProvider CreateProvider()
    {
        var collection = new ServiceCollection();
        var client = new DiscordSocketClient(new DiscordSocketConfig
        {
            LogLevel = LogSeverity.Verbose
        });
        collection.AddLogging(i => {
            i.AddConsole();
        });
        collection.AddSingleton(client);
        collection.AddSingleton<InteractionService>();
        collection.AddSingleton<PluginLoader>();
        collection.AddLavaNode(node => {
            node.SelfDeaf = false;
            node.Hostname = Environment.GetEnvironmentVariable("AIDONEUS_LAVA_HOST") ?? "localhost";
        });
        return collection.BuildServiceProvider();
    }

    async Task RunAsync(string[] args)
    {
        var version = Assembly.GetEntryAssembly()?.GetName().Version;
        if (version == null) {
            Console.WriteLine("FATAL: Could not get version");
            return;
        }
        Console.WriteLine($"Aidoneus v{version.Major}.{version.Minor}.{version.Build}");
        var token = Environment.GetEnvironmentVariable("AIDONEUS_TOKEN");
        if (token == null) {
            Console.WriteLine("FATAL: AIDONEUS_TOKEN not set");
            return;
        }

        var targetGuild = Environment.GetEnvironmentVariable("AIDONEUS_GUILD");
        if (targetGuild == null) {
            Console.WriteLine("FATAL: AIDONEUS_GUILD not set");
            return;
        }

        var client = _serviceProvider.GetRequiredService<DiscordSocketClient>();
        await client.LoginAsync(TokenType.Bot, token);
        await client.StartAsync();

        var interactionService = _serviceProvider.GetRequiredService<InteractionService>();
        var pluginLoader = _serviceProvider.GetRequiredService<PluginLoader>();
        pluginLoader.LoadAssemblies("/plugins");
        pluginLoader.RunInitalizers();

        await interactionService.AddModulesAsync(Assembly.GetExecutingAssembly(), _serviceProvider);
        foreach (var plugin in pluginLoader.LoadedPlugins) {
            var loaded = await interactionService.AddModulesAsync(plugin.Assembly, _serviceProvider);
            Console.WriteLine($"Loaded {loaded.Count()} modules from {plugin.Assembly.GetName().Name}");
        }

        
        
        
        // register command listeners
        client.InteractionCreated += async (x) => {
            var ctx = new SocketInteractionContext(client, x);
            var commandResult = await interactionService.ExecuteCommandAsync(ctx, _serviceProvider);
            if (commandResult.Error != null) {
                Console.WriteLine(commandResult.Error.Value.ToString());
            }

        };

        client.Ready += async () => {
            Console.WriteLine("Client ready!");
            Console.WriteLine("Registering commands..");
            var registered = await interactionService.RegisterCommandsToGuildAsync(ulong.Parse(targetGuild));
            Console.WriteLine($"Registered {registered.Count()} commands");
        };
        await Task.Delay(Timeout.Infinite); 
    }
}
