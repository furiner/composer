using composer.Editor.Screens.Select.Carousel;
using composer.Editor.Tests.Resources;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;
using osu.Game.Tests.Visual;
using osuTK;

namespace composer.Editor.Tests.Visual.Select
{
    public partial class TestSceneBeatmapCard : OsuTestScene
    {
        [Test]
        public void Card()
        {
            var info = TestResources.CreateTestBeatmapSetInfo(null,
                new[] { new OsuRuleset().RulesetInfo, new TaikoRuleset().RulesetInfo, new CatchRuleset().RulesetInfo, new ManiaRuleset().RulesetInfo });

            var cards = new List<BeatmapCard>();
            BeatmapCard? selectedCard = null;
            
            AddStep("clear screen", Clear);
            AddStep("add cards", () =>
            {
                var flow = new FillFlowContainer
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = 720,
                    Direction = FillDirection.Vertical,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Spacing = new Vector2(8)
                };
                
                Add(flow);
                
                foreach (var beatmap in info.Beatmaps)
                {
                    var card = new BeatmapCard(beatmap)
                    {
                        RelativeSizeAxes = Axes.X,
                        Height = 45,
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre
                    };
                    
                    flow.Add(card);
                    cards.Add(card);
                }
            });

            AddStep("select random", () =>
            {
                if (selectedCard != null)
                    selectedCard.State.Value = false;

                var card = cards[RNG.Next(cards.Count)];
                card.State.Value = true;
                selectedCard = card;
            });
        }
    }
}
