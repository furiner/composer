using composer.Desktop.Online;
using composer.Editor;

namespace composer.Desktop
{
    public partial class ComposerDesktop : EditorGame
    {
        public ComposerDesktop()
        {
            
        }
        
        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            LoadComponentAsync(new DiscordRichPresence());
        }
    }
}
