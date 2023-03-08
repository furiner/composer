// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using System.Diagnostics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Utils;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Screens.Select.Carousel;
using osuTK.Graphics;

namespace composer.Editor.Screens.Select.Carousel
{
    public partial class DrawableCarouselBeatmapSetCard : DrawableCarouselItem
    {
        public const float HEIGHT = 80;
        
        public IEnumerable<DrawableCarouselItem> DrawableBeatmaps => beatmapContainer?.IsLoaded != true ? Enumerable.Empty<DrawableCarouselItem>() : beatmapContainer.AliveChildren;

        private Container<DrawableCarouselItem>? beatmapContainer;

        private BeatmapSetInfo beatmapSet = null!;

        private Task? beatmapsLoadTask;
        
        [Resolved]
        private BeatmapManager manager { get; set; } = null!;

        protected override void FreeAfterUse()
        {
            base.FreeAfterUse();

            Item = null;
            ClearTransforms();
        }

        protected override void Update()
        {
            base.Update();
            
            Debug.Assert(Item != null);
            
            if (!Item.Visible) return;

            var targetY = Item.CarouselYPosition;

            if (Precision.AlmostEquals(targetY, Y))
                Y = targetY;
            else
                Y = (float) Interpolation.Lerp(targetY, Y, Math.Exp(-0.01 * Time.Elapsed));
        }

        protected override void UpdateItem()
        {
            base.UpdateItem();
            
            Content.Clear();

            beatmapContainer = null;
            beatmapsLoadTask = null;
            
            if (Item == null)
                return;

            beatmapSet = ((CarouselBeatmapSet) Item).BeatmapSet;

            DelayedLoadWrapper background;
            DelayedLoadWrapper mainFlow;
            
            Header.Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Masking = true,
                CornerRadius = 5,
                Children = new Drawable[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = OsuColour.Gray(13),
                    },
                    background = new DelayedLoadWrapper(() => new BeatmapSetPanelBackground(manager.GetWorkingBeatmap(beatmapSet.Beatmaps.FirstOrDefault()))
                    {
                        RelativeSizeAxes = Axes.Both
                    }, 300)
                    {
                        RelativeSizeAxes = Axes.Both
                    },
                    mainFlow = new DelayedLoadWrapper(() => new BeatmapSetPanelContent(beatmapSet), 100)
                    {
                        RelativeSizeAxes = Axes.Both
                    } 
                }
            };

            Header.Masking = false;
            
            background.DelayedLoadComplete += fadeContentIn;
            mainFlow.DelayedLoadComplete += fadeContentIn;
        }

        private void fadeContentIn(Drawable d) => d.FadeInFromZero(750, Easing.OutQuint);

        protected override void Deselected()
        {
            base.Deselected();
            updateBeatmapYPositions();
        }

        protected override void Selected()
        {
            base.Selected();
            updateBeatmapDifficulties();
        }

        protected override void ApplyState()
        {
            base.ApplyState();
            
            Header.BorderContainer.EdgeEffect = new EdgeEffectParameters
            {
                Colour = Color4.Black.Opacity(0)
            };
        }

        private void updateBeatmapDifficulties()
        {
            Debug.Assert(Item != null);

            var carouselBeatmapSet = (CarouselBeatmapSet) Item;
            var visibleBeatmaps = carouselBeatmapSet.Items.Where(c => c.Visible).ToArray();

            if (beatmapContainer != null && visibleBeatmaps.Length == beatmapContainer.Count && visibleBeatmaps.All(b => beatmapContainer.Any(c => c.Item == b)))
            {
                updateBeatmapYPositions();
            }
            else
            {
                beatmapContainer = new Container<DrawableCarouselItem>
                {
                    RelativeSizeAxes = Axes.Both,
                    ChildrenEnumerable = visibleBeatmaps.Select(c => new DrawableCarouselBeatmapCard((c as CarouselBeatmap)!))
                };

                beatmapsLoadTask = LoadComponentAsync(beatmapContainer, loaded =>
                {
                    if (beatmapContainer != loaded)
                        return;

                    Content.Child = loaded;
                    updateBeatmapYPositions();
                });
            }
        }

        private void updateBeatmapYPositions()
        {
            if (beatmapContainer == null)
                return;
            
            if (beatmapsLoadTask == null || !beatmapsLoadTask.IsCompleted)
                return;

            var yPos = DrawableCarouselBeatmapCard.CAROUSEL_BEATMAP_SPACING;
            var isSelected = Item?.State.Value == CarouselItemState.Selected;

            foreach (var panel in beatmapContainer.Children)
            {
                Debug.Assert(panel.Item != null);

                if (isSelected)
                {
                    panel.MoveToY(yPos, 800, Easing.OutQuint);
                    yPos += panel.Item.TotalHeight;
                }
                else
                    panel.MoveToY(0, 800, Easing.OutQuint);
            }
        }
    }
}
