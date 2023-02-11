using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Game.Beatmaps;

namespace composer.Editor.Screens.Select.Carousel
{
    public partial class BeatmapSetCard : CompositeDrawable
    {
        private readonly BeatmapSetInfo beatmapSet;

        private DelayedLoadWrapper background = null!;
        private DelayedLoadWrapper mainContent = null!;

        public BeatmapSetCard(BeatmapSetInfo beatmapSet)
        {
            this.beatmapSet = beatmapSet;
        }

        [BackgroundDependencyLoader]
        private void load(BeatmapManager manager)
        {
            InternalChild = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Children = new Drawable[]
                {
                    background = new DelayedLoadWrapper(() => new SetPanelBackground(manager.GetWorkingBeatmap(beatmapSet.Beatmaps.FirstOrDefault()))
                    {
                        RelativeSizeAxes = Axes.Both
                    }, 100)
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    mainContent = new DelayedLoadWrapper(() => new SetPanelContent(beatmapSet), 100)
                    {
                        RelativeSizeAxes = Axes.Both
                    } 
                }
            };

            background.DelayedLoadComplete += fadeContentIn;
            mainContent.DelayedLoadComplete += fadeContentIn;
        }
        
        private void fadeContentIn(Drawable d) => d.FadeInFromZero(750, Easing.OutQuint);
    }
}
