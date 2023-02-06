using composer.Desktop.Online;
using composer.Editor;
using osu.Framework.Platform;

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

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
