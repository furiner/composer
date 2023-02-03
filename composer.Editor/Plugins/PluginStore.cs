using System.Reflection;
using osu.Framework.Platform;

namespace composer.Editor.Plugins
{
    public class PluginStore : IDisposable
    {
        private Storage storage;

        protected readonly IDictionary<Assembly, Type> LoadedAssemblies = new Dictionary<Assembly, Type>();

        protected PluginStore(Storage storage)
        {
            this.storage = storage;

            loadFromStorage(storage.GetStorageForDirectory("plugins"));
        }

        private void loadFromStorage(Storage pluginStorage)
        {
            foreach (string plugin in pluginStorage.GetFiles(".", "*.dll"))
            {
                loadPluginFromFile(plugin);
            }
        }

        private void loadPluginFromFile(string qualifiedPath)
        {
            try
            {
                var assembly = Assembly.LoadFrom(qualifiedPath);

                if (LoadedAssemblies.ContainsKey(assembly))
                {
                    return;
                }

                LoadedAssemblies[assembly] = assembly.GetTypes().First(t => t.IsPublic && t.IsSubclassOf(typeof(Plugin)))
            } catch
            {
                // fuck you exceptions >:(
            }
        }

        public void Dispose()
        {
            
        }
    }
}