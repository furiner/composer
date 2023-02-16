using composer.Editor.Graphics;
using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Effects;
using osu.Framework.Graphics.Shapes;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace composer.Editor.Screens.Select.Carousel
{
    public partial class BeatmapCard : CompositeDrawable
    {
        private const float width_selected = 0.85f;
        private const float width_normal = 0.75f;
        private const float content_height_normal = 1f;
        private const float content_height_selected = 0.925f;

        private readonly BeatmapInfo info;

        private Container infoContainer = null!;
        private Container contentContainer = null!;
        private OsuTextFlowContainer mappedText = null!;

        // todo: move this to a more generalized form in order to let BeatmapSetCard to have the same logic.
        //       ~ Nora
        public BindableBool State { get; } = new();

        public BeatmapCard(BeatmapInfo info)
        {
            this.info = info;
        }

        [Resolved]
        private OsuColour colour { get; set; } = null!;

        private Color4 getDifficultyColour()
            => info.StarRating >= 9f ? OsuColour.Gray(26) : colour.ForStarDifficulty(info.StarRating);

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = contentContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
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
                    info.Ruleset.CreateInstance().CreateIcon().With(d =>
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
                                            new DifficultyPill(info.StarRating),
                                            mappedText = new OsuTextFlowContainer(c => c.Font = OsuFont.GetFont(Typeface.Inter, 12))
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
                                            Text = info.DifficultyName
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };

            mappedText.AddText("mapped by ");
            mappedText.AddText(info.Metadata.Author.Username, c => c.Font = OsuFont.GetFont(Typeface.Inter, 12, FontWeight.Bold));

            State.BindValueChanged(onStateChange);
        }

        private void onStateChange(ValueChangedEvent<bool> val)
        {
            if (val.NewValue)
                Selected();
            else
                Deselected();
        }

        private void Selected()
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

        private void Deselected()
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
