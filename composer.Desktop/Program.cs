// See https://aka.ms/new-console-template for more information

using composer.Editor;
using osu.Framework;
using osu.Framework.Platform;

namespace composer.Desktop
{
    public static class Program
    {
#if DEBUG
        private const string program_name = "composer-dev";
#else
            private const string program_name = "composer";
#endif
        
        public static void Main()
        {
            using (DesktopGameHost host = Host.GetSuitableDesktopHost(program_name))
            using (var game = new ComposerDesktop())
            {
                host.Run(game);
            }
        }
    }
}