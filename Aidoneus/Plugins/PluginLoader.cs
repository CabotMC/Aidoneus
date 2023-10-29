using System.Reflection;
using Aidoneus.API;
using Microsoft.Extensions.DependencyInjection;

namespace Aidoneus.Plugins;

public class PluginLoader {

    IServiceProvider _serviceProvider;
    public List<PluginSpec> LoadedPlugins = new(); 
    public List<Assembly> LoadedDependencies = new();

    public PluginLoader(IServiceProvider serviceProvider) {
        _serviceProvider = serviceProvider;
    }

    public int LoadAssemblies(string basePath) {
        var files = Directory.GetFiles(basePath);
        var assemblies = new List<Assembly>();

        foreach (var file in files) {
            if (file.EndsWith(".dll")) {
                var assembly = Assembly.LoadFile(file);
                assemblies.Add(assembly);
            }
        }
        foreach (var assembly in assemblies) {
            Type? foundType = null;
            foreach (var type in assembly.GetTypes()) {
                if (type.GetCustomAttribute<AidoneusPlugin>() != null) {
                    foundType = type;
                    break;
                }
            }
            if (foundType != null) {
                LoadedPlugins.Add(new PluginSpec(assembly, foundType));
                Console.WriteLine($"Loaded plugin {assembly.GetName().Name} v{assembly.GetName().Version}");
            } else {
                LoadedDependencies.Add(assembly);
            }
        }
        return LoadedPlugins.Count;
    }

    public void RunInitalizers() {
        foreach (var plugin in LoadedPlugins) {
            if (plugin.EntryPoint.GetInterface("Aidoneus.API.AidoneusPluginEntry") != null) {
                var instance = ActivatorUtilities.CreateInstance(_serviceProvider, plugin.EntryPoint);
                var method = plugin.EntryPoint.GetMethod("Initialize");
                method?.Invoke(instance, null);
                Console.WriteLine($"Initialized plugin {plugin.Assembly.GetName().Name} v{plugin.Assembly.GetName().Version}");
            }
        }
    }


}


public class PluginSpec {
    public Assembly Assembly {get; set;}
    public Type EntryPoint {get; set;}

    public PluginSpec(Assembly assembly, Type pluginType) {
        Assembly = assembly;
        EntryPoint = pluginType;
    }
}