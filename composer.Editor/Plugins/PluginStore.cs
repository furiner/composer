using osu.Framework.Platform;

namespace composer.Editor.Plugins
{
    public abstract class PluginStore : IDisposable
    {
        private Storage storage;
        
        protected PluginStore(Storage storage)
        {
            this.storage = storage;
        }

        public void Dispose()
        {
            
        }
    }
}