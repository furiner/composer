using composer.Editor.Screens.Select.Carousel;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;
using osu.Game.Database;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Screens.Play;

namespace composer.Editor.Screens.Select
{
    public partial class BeatmapSelect : ScreenWithBeatmapBackground
    {
        [Resolved]
        private RealmAccess realm { get; set; } = null!;

        [Resolved]
        private BeatmapManager manager { get; set; } = null!;

        private OsuScrollContainer scrollContainer = null!;
        private FillFlowContainer drawableBeatmapSets = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            Schedule(() => ApplyToBackground(b =>
            {
                b.IgnoreUserSettings.Value = true;
                b.BlurAmount.Value = 20;
                b.Colour = OsuColour.Gray(0.1f);
            }));

            AddInternal(scrollContainer = new OsuScrollContainer(Direction.Vertical)
            {
                RelativeSizeAxes = Axes.Both,
                Width = 0.5f,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Children = new Drawable[]
                {
                    drawableBeatmapSets = new FillFlowContainer
                    {
                        Direction = FillDirection.Vertical,
                        AutoSizeAxes = Axes.Y,
                        RelativeSizeAxes = Axes.X,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    }
                }
            });
        }

        private void setBeatmap(IEnumerable<BeatmapSetInfo> infos)
        {
            var info = infos.FirstOrDefault()?.Detach();
            if (info == null)
                return;

            Beatmap.Value = manager.GetWorkingBeatmap(info.Beatmaps.First());

            foreach (var setInfo in infos)
            {
                var detached = setInfo.Detach();
                drawableBeatmapSets.Add(new BeatmapSetCard(detached)
                {
                    Height = 80,
                    RelativeSizeAxes = Axes.X,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre
                });
            }
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            realm.Run(r => setBeatmap(r.All<BeatmapSetInfo>().Where(s => !s.DeletePending && !s.Protected)));
        }
    }
}
