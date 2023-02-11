using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Colour;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Beatmaps.Drawables;
using osu.Game.Graphics;

namespace composer.Editor.Screens.Select.Carousel
{
    public partial class SetPanelBackground : BufferedContainer
    {
        public SetPanelBackground(IWorkingBeatmap working)
            : base(cachedFrameBuffer: true)
        {
            RedrawOnScale = false;

            Children = new Drawable[]
            {
                new BeatmapBackgroundSprite(working)
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    FillMode = FillMode.Fill
                },
                new Box
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = ColourInfo.GradientHorizontal(OsuColour.Gray(.05f).Opacity(.9f), OsuColour.Gray(.05f).Opacity(.8f))
                }
            };
        }
    }
}
