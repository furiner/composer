// Copyright (c) ppy Pty Ltd <contact@ppy.sh>. Licensed under the MIT Licence.
// See the LICENCE file in the repository root for full licence text.

using osu.Framework.Allocation;
using osu.Framework.Audio;
using osu.Framework.Audio.Sample;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Utils;
using osu.Game.Graphics.UserInterface;
using osu.Game.Screens.Select.Carousel;

namespace composer.Editor.Screens.Select.Carousel
{
    public partial class CarouselHeader : Container
    {
        public Container BorderContainer;

        public readonly Bindable<CarouselItemState> State = new(CarouselItemState.NotSelected);

        protected override Container<Drawable> Content { get; } = new Container { RelativeSizeAxes = Axes.Both };

        public CarouselHeader()
        {
            RelativeSizeAxes = Axes.X;
            Height = DrawableCarouselItem.MAX_HEIGHT;

            InternalChild = BorderContainer = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Children = new Drawable[]
                {
                    Content,
                    new HeaderSounds()
                }
            };
        }
        
        private partial class HeaderSounds : HoverSampleDebounceComponent
        {
            private Sample? sampleHover;

            [BackgroundDependencyLoader]
            private void load(AudioManager audio)
            {
                sampleHover = audio.Samples.Get("UI/default-hover");
            }

            public override void PlayHoverSample()
            {
                if (sampleHover == null) return;

                sampleHover.Frequency.Value = 0.99 + RNG.NextDouble(0.02);
                sampleHover.Play();
            }
        }
    }
}
