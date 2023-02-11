using composer.Editor.Screens.Select;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Screens;
using osu.Game.Screens;
using osuTK;

namespace composer.Editor.Screens.Menu
{
    public partial class MainMenu : OsuScreen
    {
        public MainMenu()
        {
            AddInternal(new FillFlowContainer
            {
                RelativeSizeAxes = Axes.X,
                AutoSizeAxes = Axes.Y,
                Anchor = Anchor.Centre,
                Origin = Anchor.Centre,
                Direction = FillDirection.Vertical,
                Children = new Drawable[]
                {
                    new MenuButton
                    {
                        Size = new Vector2(272, 48),
                        Text = "Create",
                        AccentColour = Color4Extensions.FromHex(@"63F"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    new MenuButton
                    {
                        Size = new Vector2(272, 48),
                        Text = "Edit",
                        AccentColour = Color4Extensions.FromHex(@"FFD333"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Action = delegate
                        {
                            this.Push(new BeatmapSelect());
                        }
                    },
                    new MenuButton
                    {
                        Size = new Vector2(272, 48),
                        Text = "Options",
                        AccentColour = Color4Extensions.FromHex(@"3377FF"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                    },
                    new MenuButton
                    {
                        Size = new Vector2(272, 48),
                        Text = "Exit",
                        AccentColour = Color4Extensions.FromHex(@"FF3366"),
                        Anchor = Anchor.Centre,
                        Origin = Anchor.Centre,
                        Action = this.Exit // TODO: doesn't exit application
                    }
                }
            });
        }
    }
}
