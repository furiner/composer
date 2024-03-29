﻿// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Localisation;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.Sprites;
using osuTK;

namespace composer.Editor.Screens.Select.Carousel
{
    public partial class BeatmapSetPanelContent : CompositeDrawable
    {
        private readonly BeatmapSetInfo beatmapSet;

        private FillFlowContainer iconFlow = null!;

        public BeatmapSetPanelContent(BeatmapSetInfo beatmapSet)
        {
            this.beatmapSet = beatmapSet;

            RelativeSizeAxes = Axes.Both;
        }

        [Resolved]
        private OsuColour colours { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            InternalChild = new FillFlowContainer
            {
                RelativeSizeAxes = Axes.Both,
                Direction = FillDirection.Vertical,
                Padding = new MarginPadding(12),
                Children = new Drawable[]
                {
                    new OsuSpriteText
                    {
                        Text = new RomanisableString(beatmapSet.Metadata.TitleUnicode, beatmapSet.Metadata.Title),
                        Font = OsuFont.GetFont(Typeface.Inter, weight: FontWeight.SemiBold, size: 16),
                        Shadow = true
                    },
                    new OsuSpriteText
                    {
                        Text = new RomanisableString(beatmapSet.Metadata.ArtistUnicode, beatmapSet.Metadata.Artist),
                        Font = OsuFont.GetFont(Typeface.Inter, weight: FontWeight.Regular, size: 16),
                        Shadow = true,
                        Alpha = 0.5f
                    },
                    new FillFlowContainer
                    {
                        Direction = FillDirection.Horizontal,
                        AutoSizeAxes = Axes.Both,
                        Margin = new MarginPadding { Top = 8 },
                        Child = iconFlow = new FillFlowContainer
                        {
                            AutoSizeAxes = Axes.Both,
                            Origin = Anchor.CentreLeft,
                            Anchor = Anchor.CentreLeft,
                            Spacing = new Vector2(8),
                        }
                    }
                }
            };
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();
            iconFlow.ChildrenEnumerable = getDifficultyIcons();
        }
        
        private const int maximum_difficulty_lines = 20;

        private IEnumerable<Drawable> getDifficultyIcons()
        {
            var beatmaps = beatmapSet.Beatmaps.ToList();
            var rulesetGrouped = beatmaps.GroupBy(b => b.Ruleset);
            
            foreach (var info in rulesetGrouped)
            {
                var flowContainer = new FillFlowContainer
                {
                    Direction = FillDirection.Horizontal,
                    AutoSizeAxes = Axes.Both,
                    Origin = Anchor.CentreLeft,
                    Anchor = Anchor.CentreLeft,
                    Spacing = new Vector2(2)
                };
                
                flowContainer.Add(info.Key.CreateInstance().CreateIcon().With(d => d.Size = new Vector2(16)));
                var orderedBeatmaps = info.OrderBy(b => b.StarRating).ToList();

                if (orderedBeatmaps.Count >= maximum_difficulty_lines)
                {
                    flowContainer.Add(new OsuSpriteText
                    {
                        Text = orderedBeatmaps.Count.ToString(),
                        Font = OsuFont.GetFont(Typeface.Inter, weight: FontWeight.Regular, size: 16),
                        Shadow = true,
                        Origin = Anchor.CentreLeft,
                        Anchor = Anchor.CentreLeft,
                        Alpha = 0.5f
                    });
                }
                else
                {
                    foreach (var beatmap in orderedBeatmaps)
                    {
                        flowContainer.Add(new Circle
                        {
                            RelativeSizeAxes = Axes.Y,
                            Width = 4,
                            Origin = Anchor.CentreLeft,
                            Anchor = Anchor.CentreLeft,
                            Colour = colours.ForStarDifficulty(beatmap.StarRating)
                        });
                    }
                }

                yield return flowContainer;
            }
        }
    }
}
