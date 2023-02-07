using osu.Framework.Allocation;
using osu.Framework.Testing;

namespace composer.Editor.Tests.Visual
{
    public partial class TestSceneEditorGame : TestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            AddGame(new Composer());
        }
    }
}
