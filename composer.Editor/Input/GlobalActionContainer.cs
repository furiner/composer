using osu.Framework.Graphics;
using osu.Framework.Input;
using osu.Framework.Input.Bindings;
using osu.Game;

namespace composer.Editor.Input
{
    public partial class GlobalActionContainer : KeyBindingContainer<GlobalAction>
    {
        private readonly Drawable? handler;

        private readonly InputManager? parentInputManager = null;
        
        public GlobalActionContainer(OsuGameBase? game)
            : base(matchingMode: KeyCombinationMatchingMode.Modifiers)
        {
            // ReSharper disable once SuspiciousTypeConversion.Global
            if (game is IKeyBindingHandler<GlobalAction>)
                handler = game;
        }
        
        public override IEnumerable<IKeyBinding> DefaultKeyBindings
            => GlobalKeyBindings
               .Concat(SongSelectKeyBindings);

        public IEnumerable<KeyBinding> GlobalKeyBindings => new[]
        {
            new KeyBinding(InputKey.Up, GlobalAction.SelectPrevious),
            new KeyBinding(InputKey.Down, GlobalAction.SelectNext),

            new KeyBinding(InputKey.Left, GlobalAction.SelectPreviousGroup),
            new KeyBinding(InputKey.Right, GlobalAction.SelectNextGroup),

            new KeyBinding(InputKey.Space, GlobalAction.Select),
            new KeyBinding(InputKey.Enter, GlobalAction.Select),
            new KeyBinding(InputKey.KeypadEnter, GlobalAction.Select),

            new KeyBinding(InputKey.Escape, GlobalAction.Back),
            new KeyBinding(InputKey.ExtraMouseButton1, GlobalAction.Back),
        };

        public IEnumerable<KeyBinding> SongSelectKeyBindings => new[]
        {
            new KeyBinding(InputKey.F2, GlobalAction.SelectNextRandom),
            new KeyBinding(new[] { InputKey.Shift, InputKey.F2 }, GlobalAction.SelectNextRandom),
        };
        
        protected override IEnumerable<Drawable> KeyBindingInputQueue
        {
            get
            {
                // To ensure the global actions are handled with priority, this GlobalActionContainer is actually placed after game content.
                // It does not contain children as expected, so we need to forward the NonPositionalInputQueue from the parent input manager to correctly
                // allow the whole game to handle these actions.

                // An eventual solution to this hack is to create localised action containers for individual components like SongSelect, but this will take some rearranging.
                var inputQueue = parentInputManager?.NonPositionalInputQueue ?? base.KeyBindingInputQueue;

                return handler != null ? inputQueue.Prepend(handler) : inputQueue;
            }
        }
    }

    public enum GlobalAction
    {
        // Global
        SelectPrevious,
        SelectNext,
        SelectPreviousGroup,
        SelectNextGroup,
        Select,
        Back,
        
        // Song select
        SelectNextRandom,
        SelectPreviousRandom,
    }
}
