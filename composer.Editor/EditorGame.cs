using composer.Editor.Graphics.Cursor;
using composer.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Sprites;
using osu.Framework.IO.Stores;
using osu.Game;
using osuTK;
using osuTK.Graphics;

namespace composer.Editor
{
    public partial class EditorGame : OsuGameBase
    {
        private Container content = null!;

        protected override Container<Drawable> Content => content;

        protected DependencyContainer DependencyContainer = null!;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
            => DependencyContainer = new DependencyContainer(base.CreateChildDependencies(parent));

        [BackgroundDependencyLoader]
        private void load()
        {
        }

        protected override void LoadComplete()
        {
            base.LoadComplete();

            SafeAreaContainer.Child = CreateScalingContainer().WithChildren(new Drawable[]
            {
                new MenuCursorContainer
                {
                    RelativeSizeAxes = Axes.Both,
                    State = { Value = Visibility.Visible },
                    Child = content = new Container
                    {
                        RelativeSizeAxes = Axes.Both
                    }
                }
            });

            base.Add(new SpriteText
            {
                Font = new FontUsage("Montserrat", size: 40, weight: "Regular"),
                Text = "Balls and cock",
                Position = new Vector2(50),
                Colour = Color4.White,
            });
        }

        // NOTE: This is called before load()
        //       ~ Nora
        protected override void InitialiseFonts()
        {
            Resources.AddStore(new DllResourceStore(typeof(EditorResources).Assembly));
            
            AddFont(Resources, "Fonts/Montserrat/Montserrat-Bold");
            AddFont(Resources, "Fonts/Montserrat/Montserrat-SemiBold");
            AddFont(Resources, "Fonts/Montserrat/Montserrat-Medium");
            AddFont(Resources, "Fonts/Montserrat/Montserrat-Regular");
            AddFont(Resources, "Fonts/Montserrat/Montserrat-Light");

            base.InitialiseFonts();
        }
    }
}