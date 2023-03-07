﻿using System.Diagnostics;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Pooling;
using osu.Framework.Input.Events;
using osu.Game.Screens.Select.Carousel;

namespace composer.Editor.Screens.Select.Carousel
{
    public partial class DrawableCarouselItem : PoolableDrawable
    {
        public const float MAX_HEIGHT = 80;

        public override bool IsPresent => base.IsPresent || Item?.Visible == true;

        public override bool HandlePositionalInput => Item?.Visible == true;
        public override bool PropagatePositionalInputSubTree => Item?.Visible == true;
        
        public readonly CarouselHeader Header;
        
        public readonly Container<Drawable> Content;
        
        protected readonly Container MovementContainer;
        
        private CarouselItem? item;

        public CarouselItem? Item
        {
            get => item;
            set
            {
                if (item == value)
                    return;

                if (item != null)
                {
                    item.Filtered.ValueChanged -= onStateChange;
                    item.State.ValueChanged -= onStateChange;

                    Header.State.UnbindFrom(item.State);

                    if (item is CarouselGroup group)
                    {
                        foreach (var c in group.Items)
                            c.Filtered.ValueChanged -= onStateChange;
                    }
                }

                item = value;
            
                if (IsLoaded)
                    UpdateItem();
            }
        }

        public DrawableCarouselItem()
        {
            RelativeSizeAxes = Axes.X;

            Alpha = 0;

            InternalChildren = new Drawable[]
            {
                MovementContainer = new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Children = new Drawable[]
                    {
                        Header = new CarouselHeader(),
                        Content = new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                        }
                    }
                },
            };
        }
        
        public void SetMultiplicativeAlpha(float alpha) => Header.BorderContainer.Alpha = alpha;
        
        protected override void LoadComplete()
        {
            base.LoadComplete();
            UpdateItem();
        }

        protected override void Update()
        {
            base.Update();
            Content.Y = Header.Height;
        }

        protected virtual void UpdateItem()
        {
            if (Item == null)
                return;

            Scheduler.AddOnce(ApplyState);

            Item.Filtered.ValueChanged += onStateChange;
            Item.State.ValueChanged += onStateChange;

            Header.State.BindTo(Item.State);

            if (Item is CarouselGroup group)
            {
                foreach (var c in group.Items)
                    c.Filtered.ValueChanged += onStateChange;
            }
        }
        
        private void onStateChange(ValueChangedEvent<CarouselItemState> obj) => Scheduler.AddOnce(ApplyState);
        private void onStateChange(ValueChangedEvent<bool> _) => Scheduler.AddOnce(ApplyState);

        protected virtual void ApplyState()
        {
            Debug.Assert(Item != null);

            // Use the fact that we know the precise height of the item from the model to avoid the need for AutoSize overhead.
            // Additionally, AutoSize doesn't work well due to content starting off-screen and being masked away.
            Height = Item.TotalHeight;

            switch (Item.State.Value)
            {
                case CarouselItemState.NotSelected:
                    Deselected();
                    break;

                case CarouselItemState.Selected:
                    Selected();
                    break;
            }

            if (!Item.Visible)
                this.FadeOut(300, Easing.OutQuint);
            else
                this.FadeIn(250);
        }

        protected virtual void Selected()
        {
            Debug.Assert(Item != null);
        }

        protected virtual void Deselected()
        {
        }

        protected override bool OnClick(ClickEvent e)
        {
            Debug.Assert(Item != null);

            Item.State.Value = CarouselItemState.Selected;
            return true;
        }
    }
}