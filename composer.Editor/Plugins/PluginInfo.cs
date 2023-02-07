using osu.Framework.Testing;
using osu.Game.Database;
using osu.Game.Models;
using Realms;

namespace composer.Editor.Plugins
{
    [ExcludeFromDynamicCompile]
    [MapTo(("Plugin"))]
    public class PluginInfo : RealmObject, IHasGuidPrimaryKey, IHasRealmFiles, ISoftDelete, IPluginInfo
    {
        public Guid ID { get; set; }
        
        public IList<RealmNamedFileUsage> Files { get; } = null!;
        
        IEnumerable<INamedFileUsage> IHasNamedFiles.Files => Files;
        
        public bool DeletePending { get; set; }

        public string Hash { get; set; } = string.Empty;
    }
}
