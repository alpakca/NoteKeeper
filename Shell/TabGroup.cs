using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Elements;

namespace NoteKeeper.Shell
{
    // handles the selection, groupping, visualization of tabs
    internal class TabGroup
    {
        // buffer area that mouse can move without flickering (constantly changing) rendering.
        private Rectangle recentSwitchedTabsBetweenArea;

        private readonly ControlPanel panel;
        private readonly TabGroupUIElement uiElement;
        private readonly TabGroupRenderer renderer;

        public int MinimumVisualHeight { get; }
        public PopElement UIElement => uiElement;

        public Rectangle Boundaries
        {
            get;
            set
            {
                if (field.Size != value.Size)
                {
                    renderer.SizeChanged(value.Size);
                    uiElement.Texture = renderer.Target;
                }

                uiElement.Boundaries = value;
                field = value;
            }
        }

        public TabGroup(ControlPanel panel)
        {
            this.panel = panel;
            renderer = new TabGroupRenderer();
            uiElement = new TabGroupUIElement(this);

            panel.ActiveTabs.CollectionChanged += (_, _) => renderer.TabStatuesChanged(panel, this.Boundaries.Width);
        }

        public void RenderTabs()
        {
            renderer.Render(panel.DisplayedTab);
        }

        public void Holding(Point mousePosition)
        {
            // Cannot perform switching tabs if displayed tab does not exist
            if (panel.DisplayedTab == null)
            {
                return;
            }

            Tab hit = renderer.HitTab(mousePosition - Boundaries.Location);

            // if hit the displayed tab, clear the cache
            if (hit == panel.DisplayedTab)
            {
                ResetRecentSwitchedData();
            }

            // if mouse is inside buffer zone, between recent switched tab and displayed tab, don't switch it again (so it won't flicker)
            if (recentSwitchedTabsBetweenArea.Contains(mousePosition - Boundaries.Location))
            {
                return;
            }

            // if hit tab is different from displayed or mouse is out of switched tabs between area
            if (hit != null && hit != panel.DisplayedTab)
            {
                int hitTabIndex = panel.ActiveTabs.IndexOf(hit);
                int displayedTabIndex = panel.ActiveTabs.IndexOf(panel.DisplayedTab);

                int widthDifference = renderer.GetTabWidth(hit, false) - renderer.GetTabWidth(panel.DisplayedTab, true);
                if (widthDifference > 0)
                {
                    Rectangle currentBoundary = renderer.GetTabBoundary(panel.DisplayedTab);
                    if (displayedTabIndex < hitTabIndex)
                    {
                        recentSwitchedTabsBetweenArea = new Rectangle(currentBoundary.Right, currentBoundary.Y, widthDifference, currentBoundary.Height);
                    }
                    else
                    {
                        recentSwitchedTabsBetweenArea = new Rectangle(currentBoundary.X - widthDifference, currentBoundary.Y, widthDifference, currentBoundary.Height);
                    }
                }

                // change the order of tabs
                panel.ActiveTabs.RemoveAt(displayedTabIndex);
                panel.ActiveTabs.Insert(hitTabIndex, panel.DisplayedTab);
            }
        }

        public void HoldRemoved()
        {
            ResetRecentSwitchedData();
        }

        public void Selected(Point mousePosition)
        {
            Tab hit = renderer.HitTab(mousePosition - Boundaries.Location);
            if (hit != null && hit != panel.DisplayedTab)
            {
                panel.DisplayTab(hit);
            }
        }

        private void ResetRecentSwitchedData()
        {
            recentSwitchedTabsBetweenArea = new Rectangle(0, 0, -1, -1);
        }
    }
}
