using composer.Desktop.Online;
using composer.Editor;
using osu.Framework.Logging;
using osu.Framework.Platform;
using osu.Framework.Threading;

namespace composer.Desktop
{
    public partial class ComposerDesktop : Composer
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            LoadComponentAsync(new DiscordRichPresence());
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;

            ((SDL2DesktopWindow) host.Window).DragDrop += f => fileDrop(new[] { f });
        }

        private readonly List<string> importableFiles = new();
        private ScheduledDelegate importSchedule = null!;

        private void fileDrop(string[] filePaths)
        {
            lock (importableFiles)
            {
                var firstExtension = Path.GetExtension(filePaths.First());
                if (filePaths.Any(f => Path.GetExtension(f) != firstExtension)) return;
                
                importableFiles.AddRange(filePaths);
                Logger.Log($"Adding {filePaths.Length} files for import");
                
                // File drag drop operations can potentially trigger hundreds or thousands of these calls on some platforms.
                // In order to avoid spawning multiple import tasks for a single drop operation, debounce a touch.
                importSchedule?.Cancel();
                importSchedule = Scheduler.AddDelayed(handlePendingImports, 100);
            }
        }

        private void handlePendingImports()
        {
            lock (importableFiles)
            {
                Logger.Log($"Handling batch import of {importableFiles.Count} files");

                var paths = importableFiles.ToArray();
                importableFiles.Clear();

                Task.Factory.StartNew(() => Import(paths), TaskCreationOptions.LongRunning);
            }
        }
    }
}
