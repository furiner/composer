using osu.Framework.Allocation;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Screens.Play;

namespace composer.Editor.Screens.Select
{
    public partial class BeatmapSelect : ScreenWithBeatmapBackground
    {
        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        [Resolved]
        private BeatmapManager manager { get; set; } = null!;
        
        [BackgroundDependencyLoader]
        private void load()
        {
            Schedule(() => ApplyToBackground(b =>
            {
                b.IgnoreUserSettings.Value = true;
                b.BlurAmount.Value = 20;
                b.Colour = OsuColour.Gray(0.1f);
            }));
        }

        private void setBeatmap(IEnumerable<BeatmapSetInfo> infos)
        {
            var info = infos.First().Detach();
            Beatmap.Value = manager.GetWorkingBeatmap(info.Beatmaps.First());
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            realm.Run(r => setBeatmap(r.All<BeatmapSetInfo>().Where(s => !s.DeletePending && !s.Protected)));
        }
    }
}
