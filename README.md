# Aidoneus
An extensible Discord bot written in .NET

## Building
There are two ways to build Aidoneus
### Docker (recommended)
Ensure you have a recent version of Docker, and run the following comamnd:
```
docker build -t aidoneus .
```
### Vanilla
Make sure you have an SDK for .NET 7 installed, then 
you can generate plain executable files by running the following:
```
dotnet publish -c Release -o out Aidoneus/Aidoneus.csproj
```
The generatied binaries will be in the `out` directory.

## Building the music plugin
The music plugin is an example plugin that most people probably want to use. It provides basic Youtube playback through a few commands.
There is no obligation to use it, but it is provided here as an example for plugin development.

To compile the music plugin:
```
dotnet publish -c Release -o music-out Aidoneus.Plugin.Music/Aidoneus.Plugin.Music.csproj
```
The generated plugin assembly will be in the `music-out` directory.

## Usage

This bot is designed to be run in Docker, with docker-compose. An example `docker-compose.yml` can be found below:
```yml
services:
  aidoneus:
    image: aidoneus:latest
    environment:
      - AIDONEUS_TOKEN=<your bot token>
      - AIDONEUS_GUILD=<your server id>
      - AIDONEUS_LAVA_HOST=lavalink
    volumes:
      - ./plugins:/plugins
    depends_on:
      - lavalink
  lavalink:
    image: fredboat/lavalink:v3-legacy
    volumes:
      - ./lavalink/application.yml:/opt/Lavalink/application.yml
```
It can be run separately, but it is up to you to set the appropriate enviroment variables, and ensure a LavaLink server is accessable at `AIDONEUS_LAVA_HOST:2333`.

If running without Docker, make sure to set the enviroment variable `AIDONEUS_PLUGINS_DIR` to a directory containing the plugins you want to use.

### Installing Plugins

To install a plugin, drag it's DLL into the plugins folder. If using the `docker-compose.yml` given above, there will be a directory named `plugins` which they can be placed into.

You may also drag non-plugin DLLs into the plugins folder, and they will be loaded into the runtime.

## Plugin Development

Each plugin must have one class in the assembly with the `AidoneusPlugin` attribute:
```cs
[AidoneusPlugin(minVersion = "1.0.3")]
public class Plugin : AidoneusPluginEntry
{
    public void Initialize()
    {
        Console.WriteLine("Hello from my plugin!");
    }
}
```
If the plugin implements the `AidoneusPluginEntry` interface, the `Initialize` method will be called after all plugin assemblies have been loaded. It is NOT safe to use any 
injected dependencies until Initalize() is called.

Your plugin will be searched by Discord.NET's `InteractionService`. Visit [their website](https://discordnet.dev/guides/int_framework/intro.html) for more information about developing slash commands with `InteractionService`.

