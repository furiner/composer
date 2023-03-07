using composer.Editor.Screens.Select.Carousel;
using composer.Editor.Tests.Resources;
using NUnit.Framework;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;
using osu.Game.Screens.Select.Carousel;
using osu.Game.Tests.Visual;
using osuTK;

namespace composer.Editor.Tests.Visual.Select
{
    public partial class TestSceneBeatmapSetCard : OsuTestScene
    {
        [TestCase(null)]
        [TestCase(20)]
        [TestCase(40)]
        [TestCase(80)]
        public void TestBeatmaps(int? amount = null)
        {
            AddStep("clear screen", Clear);
            AddStep("add card", () =>
            {
                var beatmap = TestResources.CreateTestBeatmapSetInfo(amount,
                    new[] { new OsuRuleset().RulesetInfo, new TaikoRuleset().RulesetInfo, new CatchRuleset().RulesetInfo, new ManiaRuleset().RulesetInfo });

                var item = new CarouselBeatmapSet(beatmap);
                Add(new DrawableCarouselBeatmapSetCard
                {
                    Size = new Vector2(720, 80),
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Item = item
                });
            });
        }
    }
}
