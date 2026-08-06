using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Input;
using System;

#nullable enable

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabOrganizeManager
    {
        private readonly TabsHeaderLayoutCalculator layoutCalculator;
        private readonly TabsHeaderRenderer renderer;

        private readonly TabCollection _activeTabs;
        private Tab? _displayedTab;
        private Tab? _hoveredTab;

        private Rectangle _switchedTabsUnion;

        // belongs to hovered tab's close button
        private bool _closeButtonHovered;
        private bool _closeButtonHeld;

        public TabsHeaderStyleProvider StyleProvider { get; }
        public Rectangle ScreenBounds
        {
            get => layoutCalculator.ScreenBounds;
            set => layoutCalculator.ScreenBounds = value;
        }

        public event Action<Tab>? DisplayedTabChanged;

        public TabOrganizeManager(TabCollection activeTabs)
        {
            _activeTabs = activeTabs;
            StyleProvider = new TabsHeaderStyleProvider();

            layoutCalculator = new TabsHeaderLayoutCalculator(StyleProvider);
            renderer = new TabsHeaderRenderer(StyleProvider, layoutCalculator);
        }

        public void DisplayedTabUpdated(Tab? newTab)
        {
            _displayedTab = newTab;
        }

        public void Render()
        {
            renderer.Render(_activeTabs, _activeTabs.IndexOf(_displayedTab!), _activeTabs.IndexOf(_hoveredTab!), _closeButtonHovered, _closeButtonHeld);
        }

        public void MousePositionChangeUpdate(PopWindowMouseEventArgs e)
        {
            _hoveredTab = FindHoveredTab(e.MousePosition);

            if (_hoveredTab == null)
            {
                _closeButtonHovered = false;
                return;
            }

            TabLayout hoveredTabLayout = layoutCalculator.GetTabLayout(_activeTabs, _activeTabs.IndexOf(_hoveredTab), _activeTabs.IndexOf(_displayedTab!));
            _closeButtonHovered = hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition);

            if (e.CurrentButtons.HasLeft)
            {
                HandleTabSwitching(e.MousePosition);
            }
        }

        public void MouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (_hoveredTab == null)
            {
                return;
            }

            int hoveredTabIndex = _activeTabs.IndexOf(_hoveredTab);
            int displayedTabIndex = _activeTabs.IndexOf(_displayedTab!);
            TabLayout hoveredTabLayout = layoutCalculator.GetTabLayout(_activeTabs, hoveredTabIndex, displayedTabIndex);
            
            if (e.FirstTimeButtons.HasLeft)
            {
                if (hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition))
                {
                    _closeButtonHeld = true;
                }
                else if (hoveredTabLayout.BoxBounds.Contains(e.MousePosition) && hoveredTabIndex != displayedTabIndex)
                {
                    _closeButtonHovered = hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition);

                    DisplayedTabChanged?.Invoke(_hoveredTab);
                }
            }
            else if (e.ReleasedButtons.HasLeft)
            {
                if (_closeButtonHeld && hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition))
                {
                    _closeButtonHeld = false;
                    _activeTabs.RemoveWithAskingSave(_hoveredTab);

                    MousePositionChangeUpdate(e); // update hovered statues after remove
                }

                ResetSwitchRect();
            }
        }

        private void HandleTabSwitching(Point mousePosition)
        {
            if (_displayedTab == null || _hoveredTab == _displayedTab)
            {
                ResetSwitchRect();
                return;
            }

            if (_switchedTabsUnion.Contains(mousePosition))
            {
                return;
            }

            if (_hoveredTab != null)
            {
                int hoveredTabIndex = _activeTabs.IndexOf(_hoveredTab);
                int displayedTabIndex = _activeTabs.IndexOf(_displayedTab);

                Rectangle hoveredTabBox = layoutCalculator.GetTabLayout(_activeTabs, hoveredTabIndex, displayedTabIndex).BoxBounds;
                Rectangle displayedTabBox = layoutCalculator.GetTabLayout(_activeTabs, displayedTabIndex, displayedTabIndex).BoxBounds;

                int hoverDisplayWidthDiff = hoveredTabBox.Width - displayedTabBox.Width;
                if (hoverDisplayWidthDiff > 0)
                {
                    int switchBoxX = displayedTabIndex < hoveredTabIndex ? displayedTabBox.Right : hoveredTabBox.Right - hoverDisplayWidthDiff;
                    _switchedTabsUnion = new Rectangle(switchBoxX, hoveredTabBox.Y, hoverDisplayWidthDiff, hoveredTabBox.Height);
                }

                _activeTabs.Swap(hoveredTabIndex, displayedTabIndex);
            }
        }

        private void ResetSwitchRect()
        {
            _switchedTabsUnion = new Rectangle(0, 0, -1, -1);
        }

        private Tab? FindHoveredTab(Point mousePosition)
        {
            int displayedTabIndex = _activeTabs.IndexOf(_displayedTab!);
            for (int i = 0; i < _activeTabs.Count; i++)
            {
                TabLayout layout = layoutCalculator.GetTabLayout(_activeTabs, i, displayedTabIndex);

                if (layout.BoxBounds.Contains(mousePosition))
                {
                    return _activeTabs[i];
                }
            }

            return null;
        }
    }
}
