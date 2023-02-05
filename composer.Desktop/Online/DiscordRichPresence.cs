using DiscordRPC;
using DiscordRPC.Message;
using osu.Framework.Allocation;
using osu.Framework.Graphics;

namespace composer.Desktop.Online
{
    public partial class DiscordRichPresence : Component
    {
        private const string application_id = "1071771476409192448";
        private DiscordRpcClient client = null!;

        public readonly RichPresence CurrentPresence = new RichPresence()
            { };

        [BackgroundDependencyLoader]
        private void load()
        {
            client = new DiscordRpcClient(application_id);

            client.OnReady += onReady;

            client.Initialize();
        }
        
        private void onReady(object sender, ReadyMessage readyEvent) {
            
        }
    }
}
