using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Screens;
using osuTK;

namespace composer.Editor.Screens.Menu
{
    public partial class MainMenu : Screen
    {
        public MainMenu()
        {
            AddInternal(new MenuButton
            {
                Size = new Vector2(272, 48),
                Text = "Create",
                AccentColour = Color4Extensions.FromHex(@"63F"),
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
            });
        }
    }
}
