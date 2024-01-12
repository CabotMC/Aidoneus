using System.ComponentModel.Design;
using System.Diagnostics;
using System.Reflection;
using System.Runtime.InteropServices;
using Aidoneus.API;
using Aidoneus.API.Persistence;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

namespace Aidoneus.Plugins;

public class PluginLoader {

    IServiceProvider _serviceProvider;
    public List<PluginSpec> LoadedPlugins = new(); 
    public List<Assembly> LoadedDependencies = new();
    ILogger<PluginLoader> _logger;

    public PluginLoader(IServiceProvider serviceProvider, ILogger<PluginLoader> logger) {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    public int LoadAssemblies(string basePath) {
        var configFolder = Path.Combine(basePath, "config");
        if (!Directory.Exists(configFolder)) {
            Directory.CreateDirectory(configFolder);
        }
        
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
                var attr = type.GetCustomAttribute<AidoneusPlugin>();
                if (attr != null) {
                    // halt process if the plugin is not compatible with the current version
                    if (attr.AsVersion() > Assembly.GetEntryAssembly()?.GetName().Version) {
                        _logger.LogCritical($"Plugin {assembly.GetName().Name} v{assembly.GetName().Version} is not compatible with this version of Aidoneus");
                        Environment.Exit(1);
                    }
                    foundType = type;
                    break;
                }
            }
            if (foundType != null)
            {
                var configName = (assembly.GetName().Name ??
                                  ((GuidAttribute) assembly.GetCustomAttributes(typeof(GuidAttribute), true)[0]).Value)
                            + ".json";
                LoadedPlugins.Add(new PluginSpec(assembly, foundType, Path.Combine(configFolder, configName)));
                _logger.LogInformation($"Loaded plugin {assembly.GetName().Name} v{assembly.GetName().Version}");
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
                _logger.LogInformation($"Initialized plugin {plugin.Assembly.GetName().Name} v{plugin.Assembly.GetName().Version}");
                
                // create persistence provider

                var provider = new FileBackedPluginStorage(plugin.ConfigFile);
                plugin.PersistenceProvider = provider;
                var persistenceField = plugin.EntryPoint.GetField("_persistence", BindingFlags.NonPublic | BindingFlags.Instance);
                persistenceField?.SetValue(instance, provider);
                _logger.LogInformation(
                    $"Initialized storage provider for plugin {plugin.Assembly.GetName().Name} v{plugin.Assembly.GetName().Version} in file {plugin.ConfigFile}"
                    );
            }
        }
    }
}


public class PluginSpec {
    public Assembly Assembly {get; set;}
    public Type EntryPoint {get; set;}
    
    public string ConfigFile {get; set;}
    
    public IPluginPersistenceProvider? PersistenceProvider {get; set;}

    public PluginSpec(Assembly assembly, Type pluginType, string configFile) {
        Assembly = assembly;
        EntryPoint = pluginType;
        ConfigFile = configFile;
    }
}