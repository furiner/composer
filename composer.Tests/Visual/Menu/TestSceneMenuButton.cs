using composer.Editor.Screens.Menu;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Testing;
using osuTK;

namespace composer.Tests.Visual.Menu
{
    public partial class TestSceneMenuButton : TestScene
    {
        public TestSceneMenuButton()
        {
            Add(new MenuButton
            {
                Size = new Vector2(272, 48),
                Text = "Create",
                AccentColour = Color4Extensions.FromHex(@"63F"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Scale = new Vector2(2),
                Shear = new Vector2(0.2f, 0)
            });
        }
    }
}
