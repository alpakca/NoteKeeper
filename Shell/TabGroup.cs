using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Elements;
using PopUpUI.Input;
using PopUpUI.Surface;
using System.Collections.Generic;

namespace NoteKeeper.Shell
{
    // handles the selection, groupping, visualization of tabs
    internal class TabGroup : PopElement, ISelectable, IHoldable
    {
        private bool isHeld;
        private Rectangle recentSwitchedTabsBetweenArea;

        private readonly TextureDrawInfo drawInfo;

        private readonly ControlPanel panel;
        private readonly TabGroupRenderer renderer;

        public bool IsHeld => isHeld;

        public SelectablePacket SelectablePacket { get; } = new SelectablePacket();
        public HoldablePacket HoldPacket { get; } = new HoldablePacket();

        public int MinimumVisualHeight { get; }

        public override Rectangle Boundaries
        { 
            set
            {
                if (base.Boundaries.Size != value.Size)
                {
                    renderer.SizeChanged(value.Size);
                    drawInfo.Texture = renderer.Target;
                }

                drawInfo.DestinationArea = value;
                base.Boundaries = value;
            }
        }

        public TabGroup(ControlPanel panel)
        {
            this.panel = panel;
            drawInfo = new TextureDrawInfo();
            renderer = new TabGroupRenderer();

            panel.ActiveTabs.CollectionChanged += (_, _) => renderer.TabStatuesChanged(panel, this.Boundaries.Width);
        }

        public override void RenderUpdate()
        {
            renderer.Render(panel.DisplayedTab);
        }

        public override IEnumerable<UIDrawInfo> GetDrawInfos()
        {
            return [drawInfo];
        }

        public void OnHolding(PopWindowMouseEventArgs e)
        {
            Tab hit = renderer.HitTab(e.MousePosition - Boundaries.Location);

            // if hit the displayed tab, clear the cache
            if (hit == panel.DisplayedTab)
            {
                ResetRecentSwitchedData();
            }

            // if mouse is inside buffer zone, between recent switched tab and displayed tab, don't switch it again (so it won't flicker)
            if (recentSwitchedTabsBetweenArea.Contains(e.MousePosition - Boundaries.Location))
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

        public void OnHoldStateChange(PopWindowMouseEventArgs e, bool startedHolding)
        {
            isHeld = startedHolding;

            if (!startedHolding)
            {
                ResetRecentSwitchedData();
            }
        }

        public void OnSelect(PopWindowMouseEventArgs e)
        {
            Tab hit = renderer.HitTab(e.MousePosition - Boundaries.Location);
            if (hit != null && hit != panel.DisplayedTab)
            {
                panel.DisplayTab(hit);
            }
        }


        private void ResetRecentSwitchedData()
        {
            recentSwitchedTabsBetweenArea = new Rectangle(0, 0, -1, -1);
        }

        public void OnSelectingOutside(PopWindowMouseEventArgs e)
        {
        }

        public bool CanBeHeld(Point point)
        {
            return base.InsideBoundaries(point);
        }

        public bool CanBeSelected(Point point)
        {
            return base.InsideBoundaries(point);
        }
    }
}
