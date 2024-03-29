﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

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
    public partial class BeatmapSetPanelBackground : BufferedContainer
    {
        public BeatmapSetPanelBackground(IWorkingBeatmap working)
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
                    Colour = ColourInfo.GradientHorizontal(OsuColour.Gray(.05f).Opacity(.8f), OsuColour.Gray(.05f).Opacity(.6f))
                }
            };
        }
    }
}
