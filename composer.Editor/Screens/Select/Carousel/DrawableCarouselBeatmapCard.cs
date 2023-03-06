using composer.Editor.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Events;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Screens.Select.Carousel;
using osuTK;
using osuTK.Graphics;

namespace composer.Editor.Screens.Select.Carousel
{
    public partial class DrawableCarouselBeatmapCard : DrawableCarouselItem
    {
        public const float CAROUSEL_BEATMAP_SPACING = 5;

        public const float HEIGHT = height + CAROUSEL_BEATMAP_SPACING;

        private const float height = 45;

        private const float width_selected = 0.85f;
        private const float width_normal = 0.75f;
        private const float content_height_normal = 1f;
        private const float content_height_selected = 0.925f;

        private Action<BeatmapInfo>? selectRequested;

        private readonly BeatmapInfo beatmapInfo;

        private Container infoContainer = null!;
        private Container contentContainer = null!;
        private OsuTextFlowContainer mappedText = null!;

        public DrawableCarouselBeatmapCard(CarouselBeatmap panel)
        {
            beatmapInfo = panel.BeatmapInfo;
            Item = panel;
        }

        [Resolved]
        private OsuColour colour { get; set; } = null!;

        private Color4 getDifficultyColour()
            => beatmapInfo.StarRating >= 9f ? OsuColour.Gray(26) : colour.ForStarDifficulty(beatmapInfo.StarRating);

        [BackgroundDependencyLoader(true)]
        private void load(BeatmapSelect? select)
        {
            Header.Height = height;

            if (select != null)
            {
                selectRequested = b => select.FinaliseSelection(b);
            }

            Header.Child = contentContainer = new Container
            {
                RelativeSizeAxes = Axes.X,
                Height = height,
                Width = width_normal,
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                Masking = true,
                CornerRadius = 5,
                Children = new[]
                {
                    new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = getDifficultyColour()
                    },
                    beatmapInfo.Ruleset.CreateInstance().CreateIcon().With(d =>
                    {
                        d.Size = new Vector2(20);
                        d.Anchor = Anchor.CentreLeft;
                        d.Origin = Anchor.CentreLeft;
                        d.X = 7;
                        d.Colour = ComposerColour.ForegroundTextColourFor(getDifficultyColour(), 13);
                    }),
                    infoContainer = new Container
                    {
                        RelativeSizeAxes = Axes.Both,
                        Height = content_height_normal,
                        Width = 0.94f,
                        Anchor = Anchor.TopRight,
                        Origin = Anchor.TopRight,
                        Masking = true,
                        CornerRadius = 5,
                        // Fixes weird vertical scissoring issue with o!f
                        Y = -0.2f,
                        Children = new Drawable[]
                        {
                            new Box
                            {
                                RelativeSizeAxes = Axes.Both,
                                Colour = OsuColour.Gray(10)
                            },
                            new FillFlowContainer
                            {
                                Direction = FillDirection.Vertical,
                                Padding = new MarginPadding { Vertical = 4, Horizontal = 8 },
                                Spacing = new Vector2(0, 4),
                                Anchor = Anchor.CentreLeft,
                                Origin = Anchor.CentreLeft,
                                RelativeSizeAxes = Axes.Both,
                                Children = new Drawable[]
                                {
                                    new FillFlowContainer
                                    {
                                        Direction = FillDirection.Horizontal,
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Spacing = new Vector2(8),
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Children = new Drawable[]
                                        {
                                            new DifficultyPill(beatmapInfo.StarRating),
                                            mappedText = new OsuTextFlowContainer(c => c.Font = OsuFont.GetFont(Typeface.Inter, 12, FontWeight.Regular))
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                AutoSizeAxes = Axes.Y,
                                                Colour = OsuColour.Gray(153)
                                            }
                                        }
                                    },
                                    new Container
                                    {
                                        RelativeSizeAxes = Axes.X,
                                        AutoSizeAxes = Axes.Y,
                                        Anchor = Anchor.CentreLeft,
                                        Origin = Anchor.CentreLeft,
                                        Child = new OsuSpriteText
                                        {
                                            Font = OsuFont.GetFont(Typeface.Inter, weight: FontWeight.SemiBold),
                                            Text = beatmapInfo.DifficultyName
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            mappedText.AddText("mapped by ");
            mappedText.AddText(beatmapInfo.Metadata.Author.Username, c => c.Font = OsuFont.GetFont(Typeface.Inter, 12, FontWeight.Bold));
        }

        protected override void Selected()
        {
            contentContainer.ResizeWidthTo(width_selected, 500, Easing.OutQuint);
            infoContainer.ResizeHeightTo(content_height_selected, 500, Easing.OutQuint);
            contentContainer.EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = getDifficultyColour().Opacity(0.25f),
                Radius = 12,
                Roundness = 5,
            };
        }

        protected override void Deselected()
        {
            contentContainer.ResizeWidthTo(width_normal, 500, Easing.OutQuint);
            infoContainer.ResizeHeightTo(content_height_normal, 500, Easing.OutQuint);
            contentContainer.EdgeEffect = new EdgeEffectParameters
            {
                Type = EdgeEffectType.Glow,
                Colour = getDifficultyColour().Opacity(0.25f),
                Radius = 0,
                Roundness = 5,
            };
        }

        protected override bool OnClick(ClickEvent e)
        {
            if (Item?.State.Value == CarouselItemState.Selected)
                selectRequested?.Invoke(beatmapInfo);

            return base.OnClick(e);
        }

        private partial class DifficultyPill : CompositeDrawable
        {
            private readonly double starRating;

            public DifficultyPill(double starRating)
            {
                this.starRating = starRating;
                AutoSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load(OsuColour colour)
            {
                var col = starRating >= 9f ? OsuColour.Gray(26) : colour.ForStarDifficulty(starRating);

                InternalChild = new CircularContainer
                {
                    AutoSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Masking = true,
                    Children = new Drawable[]
                    {
                        new Box
                        {
                            RelativeSizeAxes = Axes.Both,
                            Colour = col
                        },
                        new OsuSpriteText
                        {
                            Font = OsuFont.GetFont(Typeface.Inter, 12, FontWeight.Bold),
                            Text = starRating.ToString("N2"),
                            Margin = new MarginPadding { Horizontal = 8 },
                            Colour = ComposerColour.ForegroundTextColourFor(col, 13)
                        }
                    }
                };
            }
        }
    }
}
