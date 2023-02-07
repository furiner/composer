using composer.Editor.Graphics.Cursor;
using composer.Editor.Screens.Menu;
using composer.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.IO.Stores;
using osu.Framework.Screens;
using osu.Game;

namespace composer.Editor
{
    public partial class Composer : OsuGameBase
    {
        private Container content = null!;

        protected override Container<Drawable> Content => content;

        protected ScreenStack ScreenStack = null!;

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
                        RelativeSizeAxes = Axes.Both,
                        Child = ScreenStack = new ScreenStack(new MainMenu())
                    }
                }
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