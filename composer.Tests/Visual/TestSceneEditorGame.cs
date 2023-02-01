using composer.Editor;
using osu.Framework.Allocation;
using osu.Framework.Testing;

namespace composer.Tests.Visual
{
    public partial class TestSceneEditorGame : TestScene
    {
        [BackgroundDependencyLoader]
        private void load()
        {
            AddGame(new EditorGame());
        }
    }
}
