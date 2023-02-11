using composer.Editor.Screens.Select.Carousel;
using composer.Editor.Tests.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Game.Rulesets.Catch;
using osu.Game.Rulesets.Mania;
using osu.Game.Rulesets.Osu;
using osu.Game.Rulesets.Taiko;
using osu.Game.Tests.Visual;
using osuTK;

namespace composer.Editor.Tests.Visual.Select
{
    public partial class TestSceneBeatmapSetCard : OsuTestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            Add(new BeatmapSetCard(TestResources.CreateTestBeatmapSetInfo(null,
                new[] { new OsuRuleset().RulesetInfo, new TaikoRuleset().RulesetInfo, new CatchRuleset().RulesetInfo, new ManiaRuleset().RulesetInfo }))
            {
                Size = new Vector2(720, 80),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre
            });
        }
    }
}
