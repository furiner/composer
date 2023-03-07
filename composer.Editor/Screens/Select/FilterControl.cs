using osu.Framework.Allocation;
using osu.Framework.Bindables;
using osu.Framework.Graphics;
using osu.Framework.Graphics.Containers;
using osu.Framework.Input.Events;
using osu.Game.Configuration;
using osu.Game.Graphics;
using osu.Game.Graphics.Containers;
using osu.Game.Graphics.Sprites;
using osu.Game.Graphics.UserInterface;
using osu.Game.Resources.Localisation.Web;
using osu.Game.Rulesets;
using osu.Game.Screens.Select;
using osu.Game.Screens.Select.Filter;
using osuTK;

namespace composer.Editor.Screens.Select
{
    public partial class FilterControl : Container
    {
        public const float HEIGHT = 2 * side_margin + 85;
        private const float side_margin = 20;

        public Action<FilterCriteria>? FilterChanged;

        public Bindable<string> CurrentTextSearch => searchTextBox.Current;

        // TODO: our own custom implementation of a dropdown.
        private OsuDropdown<SortMode> sortDropdown = null!;

        private Bindable<SortMode> sortMode = null!;

        private SeekLimitedSearchTextBox searchTextBox = null!;

        public FilterCriteria CreateCriteria()
        {
            var query = searchTextBox.Text;

            var criteria = new FilterCriteria
            {
                Sort = sortMode.Value,
            };

            // criteria.RulesetCriteria = ruleset.Value.CreateInstance().CreateRulesetFilterCriteria();

            FilterQueryParser.ApplyQueries(criteria, query);
            return criteria;
        }

        public override bool ReceivePositionalInputAt(Vector2 screenSpacePos) =>
            base.ReceivePositionalInputAt(screenSpacePos) || sortDropdown.ReceivePositionalInputAt(screenSpacePos);

        [BackgroundDependencyLoader(permitNulls: true)]
        private void load(IBindable<RulesetInfo> parentRuleset, OsuConfigManager config)
        {
            sortMode = config.GetBindable<SortMode>(OsuSetting.SongSelectSortingMode);

            Child = new Container
            {
                RelativeSizeAxes = Axes.Both,
                Padding = new MarginPadding(side_margin),
                Width = 0.5f,
                Anchor = Anchor.TopRight,
                Origin = Anchor.TopRight,
                Child = new ReverseChildIDFillFlowContainer<Drawable>
                {
                    RelativeSizeAxes = Axes.Both,
                    Spacing = new Vector2(0, 5),
                    Children = new[]
                    {
                        new Container
                        {
                            RelativeSizeAxes = Axes.Both,
                            Children = new Drawable[]
                            {
                                searchTextBox = new SeekLimitedSearchTextBox { RelativeSizeAxes = Axes.X },
                                new GridContainer
                                {
                                    Anchor = Anchor.TopRight,
                                    Origin = Anchor.TopRight,
                                    RelativeSizeAxes = Axes.X,
                                    AutoSizeAxes = Axes.Y,
                                    Y = 40,
                                    ColumnDimensions = new[]
                                    {
                                        new Dimension(GridSizeMode.AutoSize),
                                        new Dimension(GridSizeMode.Absolute, 10),
                                        new Dimension()
                                    },
                                    RowDimensions = new[] { new Dimension(GridSizeMode.AutoSize) },
                                    Content = new[]
                                    {
                                        new[]
                                        {
                                            new OsuSpriteText
                                            {
                                                Text = SortStrings.Default,
                                                Font = OsuFont.GetFont(size: 14),
                                                Margin = new MarginPadding(5),
                                                Anchor = Anchor.TopRight,
                                                Origin = Anchor.TopRight,
                                                Y = 10
                                            },
                                            Empty(),
                                            sortDropdown = new OsuDropdown<SortMode>()
                                            {
                                                RelativeSizeAxes = Axes.X,
                                                Anchor = Anchor.TopRight,
                                                Origin = Anchor.TopRight,
                                                Current = { BindTarget = sortMode },
                                                // annoying...
                                                Items = new[]
                                                {
                                                    SortMode.Artist,
                                                    SortMode.Author,
                                                    SortMode.Difficulty,
                                                    SortMode.Length,
                                                    SortMode.Source,
                                                    SortMode.Title,
                                                    SortMode.DateAdded,
                                                    SortMode.DateRanked,
                                                    SortMode.DateSubmitted,
                                                    SortMode.LastPlayed,
                                                    SortMode.BPM
                                                }
                                            },
                                            Empty()
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            };
            
            ruleset.BindTo(parentRuleset);
            ruleset.BindValueChanged(_ => updateCriteria());
            
            sortMode.BindValueChanged(_ => updateCriteria());

            searchTextBox.Current.ValueChanged += _ => updateCriteria();
            
            updateCriteria();
        }

        public void Deactivate()
        {
            searchTextBox.ReadOnly = true;
            searchTextBox.HoldFocus = false;
            if (searchTextBox.HasFocus)
                GetContainingInputManager().ChangeFocus(searchTextBox);
        }

        public void Activate()
        {
            searchTextBox.ReadOnly = false;
            searchTextBox.HoldFocus = true;
        }

        private readonly IBindable<RulesetInfo> ruleset = new Bindable<RulesetInfo>();

        private void updateCriteria() => FilterChanged?.Invoke(CreateCriteria());

        protected override bool OnClick(ClickEvent e) => true;

        protected override bool OnHover(HoverEvent e) => true;
    }
}
