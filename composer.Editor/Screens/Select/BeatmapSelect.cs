using System.Diagnostics;
using composer.Editor.Input;
using osu.Framework.Allocation;
using osu.Framework.Audio.Track;
using osu.Framework.Bindables;
using osu.Framework.Extensions.ObjectExtensions;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Graphics.Shapes;
using osu.Framework.Input.Bindings;
using osu.Framework.Input.Events;
using osu.Framework.Logging;
using osu.Framework.Screens;
using osu.Framework.Threading;
using osu.Game.Beatmaps;
using osu.Game.Graphics;
using osu.Game.Graphics.UserInterface;
using osu.Game.Overlays;
using osu.Game.Rulesets;
using osu.Game.Screens.Play;
using osu.Game.Screens.Select;

namespace composer.Editor.Screens.Select
{
    public abstract partial class BeatmapSelect : ScreenWithBeatmapBackground, IKeyBindingHandler<GlobalAction>
    {
        protected const float BACKGROUND_BLUR = 20;
        
        public FilterControl FilterControl { get; private set; } = null!;

        protected virtual bool ControlGlobalMusic => true;

        public bool BeatmapSetsLoaded => IsLoaded && Carousel.BeatmapSetsLoaded;

        private Container carouselContainer = null!;

        protected BeatmapCarousel Carousel { get; private set; } = null!;
        protected BeatmapInfo BeatmapInfo { get; private set; } = null!;

        private readonly Bindable<RulesetInfo> decoupledRuleset = new();

        private double audioFeedbackLastPlaybackTime;

        [Resolved]
        private MusicController music { get; set; } = null!;

        [Resolved]
        private BeatmapManager beatmaps { get; set; } = null!;

        [BackgroundDependencyLoader]
        private void load()
        {
            LoadComponentAsync(Carousel = new BeatmapCarousel
            {
                Anchor = Anchor.CentreRight,
                Origin = Anchor.CentreRight,
                RelativeSizeAxes = Axes.Both,
                BleedTop = FilterControl.HEIGHT,
                SelectionChanged = updateSelectedBeatmap,
                BeatmapSetsChanged = carouselBeatmapsLoaded
            }, c => carouselContainer.Child = c);

            transferRulesetValue();

            AddRangeInternal(new Drawable[]
            {
                new ResetScrollContainer(() => Carousel.ScrollToSelected())
                {
                    RelativeSizeAxes = Axes.Y,
                    Width = 250
                },
                new Container
                {
                    RelativeSizeAxes = Axes.Both,
                    Anchor = Anchor.Centre,
                    Origin = Anchor.Centre,
                    Children = new Drawable[]
                    {
                        new GridContainer // used for max width
                        {
                            RelativeSizeAxes = Axes.Both,
                            ColumnDimensions = new[]
                            {
                                new Dimension(),
                                new Dimension(GridSizeMode.Relative, 0.5f, maxSize: 850)
                            },
                            Content = new[]
                            {
                                new Drawable[]
                                {
                                    new Box
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Alpha = 0,
                                        AlwaysPresent = true
                                    },
                                    carouselContainer = new Container
                                    {
                                        RelativeSizeAxes = Axes.Both,
                                        Padding = new MarginPadding
                                        {
                                            Right = 12,
                                            Left = 104,
                                            Bottom = 12,
                                            Top = FilterControl.HEIGHT
                                        },
                                        Child = new LoadingSpinner(true) { State = { Value = Visibility.Visible } }
                                    }
                                }
                            }
                        },
                        FilterControl = new FilterControl
                        {
                            RelativeSizeAxes = Axes.X,
                            Height = FilterControl.HEIGHT,
                            FilterChanged = ApplyFilterToCarousel
                        }
                    }
                }
            });
        }

        protected virtual void ApplyFilterToCarousel(FilterCriteria criteria)
        {
            var shouldDebounce = this.IsCurrentScreen();
            
            Carousel.Filter(criteria, shouldDebounce);
        }

        private DependencyContainer dependencies = null!;

        protected override IReadOnlyDependencyContainer CreateChildDependencies(IReadOnlyDependencyContainer parent)
        {
            dependencies = new DependencyContainer(base.CreateChildDependencies(parent));

            dependencies.CacheAs(this);
            dependencies.CacheAs(decoupledRuleset);
            dependencies.CacheAs<IBindable<RulesetInfo>>(decoupledRuleset);

            return dependencies;
        }

        public void FinaliseSelection(BeatmapInfo? beatmapInfo = null, RulesetInfo? ruleset = null, Action? customStartAction = null)
        {
            if (!Carousel.BeatmapSetsLoaded)
            {
                Logger.Log($"{nameof(FinaliseSelection)} aborted as carousel beatmaps are not yet loaded");
                return;
            }

            if (ruleset != null)
                Ruleset.Value = ruleset;

            transferRulesetValue();

            // while transferRulesetValue will flush, it only does so if the ruleset changes.
            // the user could have changed a filter, and we want to ensure we are 100% up-to-date and consistent here.
            Carousel.FlushPendingFilterOperations();

            // avoid attempting to continue before a selection has been obtained.
            // this could happen via a user interaction while the carousel is still in a loading state.
            if (Carousel.SelectedBeatmapInfo == null) return;

            if (beatmapInfo != null)
                Carousel.SelectBeatmap(beatmapInfo);

            if (selectionChangedDebounce?.Completed == false)
            {
                selectionChangedDebounce.RunTask();
                selectionChangedDebounce?.Cancel(); // cancel the already scheduled task.
                selectionChangedDebounce = null;
            }

            if (customStartAction != null)
            {
                customStartAction();
                Carousel.AllowSelection = false;
            }
            else if (OnStart())
                Carousel.AllowSelection = false;
        }

        protected abstract bool OnStart();

        private ScheduledDelegate? selectionChangedDebounce;

        private void updateCarouselSelection(ValueChangedEvent<WorkingBeatmap>? e = null)
        {
            var beatmap = e?.NewValue ?? Beatmap.Value;
            if (beatmap is DummyWorkingBeatmap || !this.IsCurrentScreen()) return;

            Logger.Log($"Song select working beatmap updated to {beatmap}");

            if (!Carousel.SelectBeatmap(beatmap.BeatmapInfo, false))
            {
                // A selection may not have been possible with filters applied.

                // There was possibly a ruleset mismatch. This is a case we can help things along by updating the game-wide ruleset to match.
                if (!beatmap.BeatmapInfo.Ruleset.Equals(decoupledRuleset.Value))
                {
                    Ruleset.Value = beatmap.BeatmapInfo.Ruleset;
                    transferRulesetValue();
                }

                // Even if a ruleset mismatch was not the cause (ie. a text filter is applied),
                // we still want to temporarily show the new beatmap, bypassing filters.
                // This will be undone the next time the user changes the filter.
                var criteria = FilterControl.CreateCriteria();
                criteria.SelectedBeatmapSet = beatmap.BeatmapInfo.BeatmapSet;
                Carousel.Filter(criteria);

                Carousel.SelectBeatmap(beatmap.BeatmapInfo);
            }
        }

        private BeatmapInfo? beatmapInfoPrevious;
        private BeatmapInfo? beatmapInfoNoDebounce;
        private RulesetInfo? rulesetNoDebounce;

        private void updateSelectedBeatmap(BeatmapInfo? beatmapInfo)
        {
            if (beatmapInfo == null && beatmapInfoNoDebounce == null)
                return;

            if (beatmapInfo?.Equals(beatmapInfoNoDebounce) == true)
                return;

            beatmapInfoNoDebounce = beatmapInfo;
            performUpdateSelected();
        }

        private void updateSelectedRuleset(RulesetInfo? ruleset)
        {
            if (ruleset == null && rulesetNoDebounce == null)
                return;

            if (ruleset?.Equals(rulesetNoDebounce) == true)
                return;

            rulesetNoDebounce = ruleset;
            performUpdateSelected();
        }

        private void performUpdateSelected()
        {
            var beatmap = beatmapInfoNoDebounce;
            RulesetInfo? ruleset = rulesetNoDebounce;

            selectionChangedDebounce?.Cancel();

            if (beatmapInfoNoDebounce == null)
                run();
            else
                selectionChangedDebounce = Scheduler.AddDelayed(run, 200);

            if (beatmap?.Equals(beatmapInfoPrevious) != true)
            {
                if (beatmap != null && beatmapInfoPrevious != null && Time.Current - audioFeedbackLastPlaybackTime >= 50)
                {
                    audioFeedbackLastPlaybackTime = Time.Current;
                }

                beatmapInfoPrevious = beatmap;
            }

            void run()
            {
                // clear pending task immediately to track any potential nested debounce operation.
                selectionChangedDebounce = null;

                Logger.Log($"Song select updating selection with beatmap:{beatmap?.ID.ToString() ?? "null"} ruleset:{ruleset?.ShortName ?? "null"}");

                if (transferRulesetValue())
                {
                    // transferRulesetValue() may trigger a re-filter. If the current selection does not match the new ruleset, we want to switch away from it.
                    // The default logic on WorkingBeatmap change is to switch to a matching ruleset (see workingBeatmapChanged()), but we don't want that here.
                    // We perform an early selection attempt and clear out the beatmap selection to avoid a second ruleset change (revert).
                    if (beatmap != null && !Carousel.SelectBeatmap(beatmap, false))
                        beatmap = null;
                }

                if (selectionChangedDebounce != null)
                {
                    // a new nested operation was started; switch to it for further selection.
                    // this avoids having two separate debounces trigger from the same source.
                    selectionChangedDebounce.RunTask();
                    return;
                }

                // We may be arriving here due to another component changing the bindable Beatmap.
                // In these cases, the other component has already loaded the beatmap, so we don't need to do so again.
                if (!EqualityComparer<BeatmapInfo>.Default.Equals(beatmap, Beatmap.Value.BeatmapInfo))
                {
                    Logger.Log($"Song select changing beatmap from \"{Beatmap.Value.BeatmapInfo}\" to \"{beatmap?.ToString() ?? "null"}\"");
                    Beatmap.Value = beatmaps.GetWorkingBeatmap(beatmap);
                }

                if (this.IsCurrentScreen())
                    ensurePlayingSelected();

                updateComponentFromBeatmap(Beatmap.Value);
            }
        }

        public override void OnEntering(ScreenTransitionEvent e)
        {
            base.OnEntering(e);

            this.FadeInFromZero(250);
            FilterControl.Activate();

            beginLooping();
        }

        public override void OnResuming(ScreenTransitionEvent e)
        {
            base.OnResuming(e);

            Carousel.AllowSelection = true;

            beginLooping();

            if (Beatmap != null && !Beatmap.Value.BeatmapSetInfo.DeletePending)
            {
                updateCarouselSelection();

                updateComponentFromBeatmap(Beatmap.Value);

                if (ControlGlobalMusic)
                {
                    // restart playback on returning to song select, regardless.
                    // not sure this should be a permanent thing (we may want to leave a user pause paused even on returning)
                    music.ResetTrackAdjustments();
                    music.Play(requestedByUser: true);
                }
            }
            
            FilterControl.MoveToY(0, 400, Easing.OutQuint);
            FilterControl.FadeIn(100, Easing.OutQuint);

            this.FadeIn(250, Easing.OutQuint);
            
            FilterControl.Activate();
        }

        public override void OnSuspending(ScreenTransitionEvent e)
        {
            transferRulesetValue();

            playExitingTransition();
            base.OnSuspending(e);
        }

        public override bool OnExiting(ScreenExitEvent e)
        {
            if (base.OnExiting(e))
                return true;

            playExitingTransition();
            return false;
        }

        private void playExitingTransition()
        {
            Carousel.AllowSelection = false;

            endLooping();
            
            FilterControl.MoveToY(-120, 500, Easing.OutQuint);
            FilterControl.FadeOut(200, Easing.OutQuint);

            this.FadeOut(400, Easing.OutQuint);
            
            FilterControl.Deactivate();
        }

        private bool isHandlingLooping;

        private void beginLooping()
        {
            if (!ControlGlobalMusic)
                return;

            Debug.Assert(!isHandlingLooping);

            isHandlingLooping = true;

            ensureTrackLooping(Beatmap.Value, TrackChangeDirection.None);

            music.TrackChanged += ensureTrackLooping;
        }

        private void endLooping()
        {
            // may be called multiple times during screen exit process.
            if (!isHandlingLooping)
                return;

            music.CurrentTrack.Looping = isHandlingLooping = false;

            music.TrackChanged -= ensureTrackLooping;
        }

        private void ensureTrackLooping(IWorkingBeatmap beatmap, TrackChangeDirection changeDirection)
            => beatmap.PrepareTrackForPreview(true);

        protected override void Dispose(bool isDisposing)
        {
            base.Dispose(isDisposing);

            decoupledRuleset.UnbindAll();

            if (music.IsNotNull())
                music.TrackChanged -= ensureTrackLooping;
        }

        private void updateComponentFromBeatmap(WorkingBeatmap beatmap)
        {
            if (this.IsCurrentScreen())
            {
                ApplyToBackground(backgroundModeBeatmap =>
                {
                    backgroundModeBeatmap.Beatmap = beatmap;
                    backgroundModeBeatmap.IgnoreUserSettings.Value = true;
                    backgroundModeBeatmap.FadeColour(OsuColour.Gray(40), 250);
                    backgroundModeBeatmap.BlurAmount.Value = BACKGROUND_BLUR;
                });
            }

            // update components...
        }

        private readonly WeakReference<ITrack?> lastTrack = new(null);

        private void ensurePlayingSelected()
        {
            if (!ControlGlobalMusic)
                return;

            ITrack track = music.CurrentTrack;

            bool isNewTrack = !lastTrack.TryGetTarget(out var last) || last != track;

            if (!track.IsRunning && (music.UserPauseRequested != true || isNewTrack))
            {
                Logger.Log($"Song select decided to {nameof(ensurePlayingSelected)}");
                music.Play(true);
            }

            lastTrack.SetTarget(track);
        }

        private void carouselBeatmapsLoaded()
        {
            bindBindables();

            Carousel.AllowSelection = true;

            // If a selection was already obtained, do not attempt to update the selected beatmap.
            if (Carousel.SelectedBeatmapSet != null)
                return;

            // Attempt to select the current beatmap on the carousel, if it is valid to be selected.
            if (!Beatmap.IsDefault && Beatmap.Value.BeatmapSetInfo?.DeletePending == false && Beatmap.Value.BeatmapSetInfo?.Protected == false)
            {
                if (Carousel.SelectBeatmap(Beatmap.Value.BeatmapInfo, false))
                    return;

                // prefer not changing ruleset at this point, so look for another difficulty in the currently playing beatmap
                var found = Beatmap.Value.BeatmapSetInfo.Beatmaps.FirstOrDefault(b => b.Ruleset.Equals(decoupledRuleset.Value));

                if (found != null && Carousel.SelectBeatmap(found, false))
                    return;
            }

            // If the current active beatmap could not be selected, select a new random beatmap.
            if (!Carousel.SelectNextRandom())
            {
                // in the case random selection failed, we want to trigger selectionChanged
                // to show the dummy beatmap (we have nothing else to display).
                performUpdateSelected();
            }
        }

        private bool boundLocalBindables;

        private void bindBindables()
        {
            if (boundLocalBindables)
                return;

            // manual binding to parent ruleset to allow for delayed load in the incoming direction.
            transferRulesetValue();

            Ruleset.ValueChanged += r => updateSelectedRuleset(r.NewValue);

            decoupledRuleset.ValueChanged += r =>
            {
                bool wasDisabled = Ruleset.Disabled;

                // a sub-screen may have taken a lease on this decoupled ruleset bindable,
                // which would indirectly propagate to the game-global bindable via the `DisabledChanged` callback below.
                // to make sure changes sync without crashes, lift the disable for a short while to sync, and then restore the old value.
                Ruleset.Disabled = false;
                Ruleset.Value = r.NewValue;
                Ruleset.Disabled = wasDisabled;
            };
            decoupledRuleset.DisabledChanged += r => Ruleset.Disabled = r;

            Beatmap.BindValueChanged(updateCarouselSelection);

            boundLocalBindables = true;
        }

        private bool transferRulesetValue()
        {
            if (decoupledRuleset.Value?.Equals(Ruleset.Value) == true)
                return false;

            Logger.Log($"decoupled ruleset transferred (\"{decoupledRuleset.Value}\" -> \"{Ruleset.Value}\")");
            rulesetNoDebounce = decoupledRuleset.Value = Ruleset.Value;

            // if we have a pending filter operation, we want to run it now.
            // it could change selection (ie. if the ruleset has been changed).
            Carousel.FlushPendingFilterOperations();

            return true;
        }

        public bool OnPressed(KeyBindingPressEvent<GlobalAction> e)
        {
            if (e.Repeat)
                return false;

            if (!this.IsCurrentScreen()) return false;

            switch (e.Action)
            {
                case GlobalAction.Select:
                    FinaliseSelection();
                    return true;
            }

            return false;
        }

        public void OnReleased(KeyBindingReleaseEvent<GlobalAction> e)
        {
        }

        private partial class ResetScrollContainer : Container
        {
            private readonly Action? onHoverAction;

            public ResetScrollContainer(Action onHoverAction)
            {
                this.onHoverAction = onHoverAction;
            }

            protected override bool OnHover(HoverEvent e)
            {
                onHoverAction?.Invoke();
                return base.OnHover(e);
            }
        }
    }
}
