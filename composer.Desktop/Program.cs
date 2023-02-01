// See https://aka.ms/new-console-template for more information

using composer.Editor;
using osu.Framework;
using osu.Framework.Platform;

namespace composer.Desktop
{
    public static class Program
    {
        public static void Main()
        {
            using (DesktopGameHost host = Host.GetSuitableDesktopHost("owo"))
            using (var game = new EditorGame())
            {
                host.Run(game);
            }
        }
    }
}