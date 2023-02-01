﻿using osu.Framework.Graphics;
using osu.Framework.Platform;
using osu.Framework.Testing;
using osu.Game;

namespace composer.Tests
{
    public partial class EditorTestBrowser : OsuGameBase
    {
        protected override void LoadComplete()
        {
            base.LoadComplete();
            
            AddRange(new Drawable[]
            {
                new TestBrowser("composer.Tests"),
            });
        }

        public override void SetHost(GameHost host)
        {
            base.SetHost(host);
            host.Window.CursorState |= CursorState.Hidden;
        }
    }
}
