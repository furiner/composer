using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.UserInterface;
using osu.Framework.Input.Events;
using osu.Framework.Localisation;
using osu.Game.Graphics.Sprites;
using osuTK;
using osuTK.Graphics;

namespace composer.Editor.Screens.Menu
{
    public partial class MenuButton : Button
    {
        public LocalisableString Text
        {
            get => SpriteText?.Text ?? default;
            set => SpriteText.Text = value;
        }

        private Color4 accentColour;

        public Color4 AccentColour
        {
            get => accentColour;
            set
            {
                accentColour = value;
                LeftLine.Child.Colour = value;
                RightLine.Child.Colour = value;
            }
        }
        
        protected readonly SpriteText SpriteText;
        protected readonly Container LeftLine;
        protected readonly Container RightLine;
        protected readonly Container Background;

        public MenuButton()
        {
            AddRange(new Drawable[]
            {
                LeftLine = new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    Size = new Vector2(3, 0.6f),
                    X = -6,
                    Anchor = Anchor.CentreLeft,
                    Origin = Anchor.CentreLeft,
                    Masking = true,
                    CornerRadius = 3f,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = accentColour
                    }
                },
                RightLine = new Container
                {
                    RelativeSizeAxes = Axes.Y,
                    Size = new Vector2(3, 0.6f),
                    X = 6,
                    Anchor = Anchor.CentreRight,
                    Origin = Anchor.CentreRight,
                    Masking = true,
                    CornerRadius = 3f,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = accentColour
                    }
                },
                Background = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Masking = true,
                    CornerRadius = 5,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Child = new Box
                    {
                        RelativeSizeAxes = Axes.Both,
                        Colour = Color4Extensions.FromHex(@"0D0D0D")
                    }
                },
                SpriteText = new OsuSpriteText
                {
                    Depth = -1,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Colour = Color4Extensions.FromHex(@"999"),
                }
            });
        }

        protected override void Update()
        {
            SpriteText.Shear = -Shear;
        }

        protected override bool OnHover(HoverEvent e)
        {
            Background.Child.Colour = Color4Extensions.FromHex(@"0D0D0D");
            Background.Child.FadeColour(Color4Extensions.FromHex(@"1A1A1A"), 300, Easing.OutQuint);

            SpriteText.Colour = Color4Extensions.FromHex(@"999");
            SpriteText.FadeColour(Color4Extensions.FromHex(@"CCC"), 300, Easing.OutQuint);

            LeftLine.Width = 3;
            LeftLine.ResizeWidthTo(32, 1500, Easing.OutQuint);
            LeftLine.CornerRadius = 2.5f;
            LeftLine.TransformTo(nameof(LeftLine.CornerRadius), 5f, 300, Easing.OutQuint);
            RightLine.Width = 3;
            RightLine.ResizeWidthTo(32, 1500, Easing.OutQuint);
            RightLine.CornerRadius = 2.5f;
            RightLine.TransformTo(nameof(LeftLine.CornerRadius), 5f, 300, Easing.OutQuint);

            return base.OnHover(e);
        }

        protected override void OnHoverLost(HoverLostEvent e)
        {
            base.OnHoverLost(e);

            Background.Child.Colour = Color4Extensions.FromHex(@"1A1A1A");
            Background.Child.FadeColour(Color4Extensions.FromHex(@"0D0D0D"), 300, Easing.OutQuint);

            SpriteText.Colour = Color4Extensions.FromHex(@"CCC");
            SpriteText.FadeColour(Color4Extensions.FromHex(@"999"), 300, Easing.OutQuint);

            LeftLine.Width = 32;
            LeftLine.ResizeWidthTo(3, 300, Easing.OutQuint);
            LeftLine.CornerRadius = 5f;
            LeftLine.TransformTo(nameof(LeftLine.CornerRadius), 2.5f, 300, Easing.OutQuint);
            RightLine.Width = 32;
            RightLine.ResizeWidthTo(3, 300, Easing.OutQuint);
            RightLine.CornerRadius = 5f;
            RightLine.TransformTo(nameof(LeftLine.CornerRadius), 2.5f, 300, Easing.OutQuint);
        }

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            Background.Width = 1f;
            Background.ResizeWidthTo(0.9f, 1000, Easing.Out);
            
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            Background.Width = 0.9f;
            Background.ResizeWidthTo(1f, 300, Easing.OutQuint);
            
            base.OnMouseUp(e);
        }
    }
}
