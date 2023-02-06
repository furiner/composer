using osu.Framework.Allocation;
using osu.Framework.Extensions.Color4Extensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Cursor;
using osu.Framework.Graphics.Sprites;
using osu.Framework.Graphics.Textures;
using osu.Framework.Input.Events;
using osuTK;
using osuTK.Graphics;

namespace composer.Editor.Graphics.Cursor
{
    public partial class MenuCursorContainer : CursorContainer
    {
        protected override Drawable CreateCursor() => activeCursor = new Cursor();

        private Cursor activeCursor = null!;

        protected override bool OnMouseDown(MouseDownEvent e)
        {
            if (State.Value == Visibility.Visible)
            {
                activeCursor.Scale = new Vector2(1);
                activeCursor.ScaleTo(0.9f, 800, Easing.OutQuint);

                activeCursor.CursorSprite.Colour = Color4.White;
                activeCursor.CursorSprite.FadeColour(Color4Extensions.FromHex(@"69F"), 800, Easing.OutQuint);
            }
            
            return base.OnMouseDown(e);
        }

        protected override void OnMouseUp(MouseUpEvent e)
        {
            if (!e.HasAnyButtonPressed)
            {
                activeCursor.CursorSprite.FadeColour(Color4.White, 500, Easing.OutQuint);
                activeCursor.ScaleTo(1f, 500, Easing.OutQuint);
            }
            
            base.OnMouseUp(e);
        }

        protected override void PopIn()
        {
            activeCursor.FadeTo(1f, 250, Easing.OutQuint);
            activeCursor.ScaleTo(1f, 400, Easing.OutQuint);
        }

        protected override void PopOut()
        {
            activeCursor.FadeTo(0, 250, Easing.OutQuint);
            activeCursor.ScaleTo(0.6f, 250, Easing.In);
        }

        public partial class Cursor : Container
        {
            public Sprite CursorSprite = null!;

            public Cursor()
            {
                AutoSizeAxes = Axes.Both;
            }

            [BackgroundDependencyLoader]
            private void load(TextureStore store)
            {
                Children = new Drawable[]
                {
                    CursorSprite = new Sprite
                    {
                        Colour = Color4.White,
                        Texture = store.Get("UI/cursor")
                    }
                };
            }
        }
    }
}
