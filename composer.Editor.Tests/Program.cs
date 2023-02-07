using osu.Framework;

namespace composer.Editor.Tests
{
    public static class Program
    {
        public static void Main()
        {
            using var host = Host.GetSuitableDesktopHost("composer");
            using var game = new EditorTestBrowser();
            
            host.Run(game);
        }
    }
}
