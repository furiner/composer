using composer.Resources;
using osu.Framework.Allocation;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Graphics.Sprites;
using osu.Framework.IO.Stores;
using osu.Game;
using osuTK;
using osuTK.Graphics;

namespace composer.Editor
{
    public partial class EditorGame : OsuGameBase
    {
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
            
            Content.AddRange(new Drawable[]
            {
                new Box
                {
                    Position = new Vector2(50),
                    Colour = Color4.Red,
                    Size = new Vector2(100)
                },
                new SpriteText()
                {
                    // Montserrat-Bold
                    Font = new FontUsage("Montserrat", weight: "bold"),
                    Position = new Vector2(50),
                    Text = "cock and ball"
                }
            });
            
            // base.Add(new SpriteText
            // {
            //     Font = new FontUsage("Montserrat", weight: "Regular"),
            //     Text = "Balls and cock",
            //     Position = new Vector2(50),
            //     Colour = Color4.White,
            //     Size = new Vector2(200)
            // });
        }

        // NOTE: This is called before load()
        //       ~ Nora
        protected override void InitialiseFonts()
        {
            Resources.AddStore(new DllResourceStore(EditorResources.ResourceAssembly));

            AddFont(Resources, "Fonts/Montserrat/Montserrat-Bold");
            AddFont(Resources, "Fonts/Montserrat/Montserrat-SemiBold");
            AddFont(Resources, "Fonts/Montserrat/Montserrat-Medium");
            AddFont(Resources, "Fonts/Montserrat/Montserrat-Regular");
            AddFont(Resources, "Fonts/Montserrat/Montserrat-Light");

            base.InitialiseFonts();
        }
    }
}