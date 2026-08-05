using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Input;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabOrganizeManager
    {
        private readonly TabsHeaderLayoutCalculator layoutCalculator;
        private readonly TabsHeaderRenderer renderer;

        private readonly IList<Tab> _activeTabs;
        private Tab? _displayedTab;
        private Tab? _hoveredTab;
        private int _displayedTabIndex;
        private int _hoveredTabIndex;

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

        public TabOrganizeManager(IList<Tab> activeTabs)
        {
            _activeTabs = activeTabs;
            StyleProvider = new TabsHeaderStyleProvider();

            layoutCalculator = new TabsHeaderLayoutCalculator(StyleProvider);
            renderer = new TabsHeaderRenderer(StyleProvider, layoutCalculator);
        }

        public void DisplayedTabUpdated(Tab? newTab)
        {
            _displayedTab = newTab;
            _displayedTabIndex = _activeTabs.IndexOf(newTab!);
        }

        public void Render(IList<Tab> activeTabs, Tab? displayedTab)
        {
            renderer.Render(activeTabs, displayedTab, null, false, false);
        }

        public void MousePositionChangeUpdate(PopWindowMouseEventArgs e)
        {
            (_hoveredTab, _hoveredTabIndex) = FindHoveredTab(e.MousePosition);

            if (_hoveredTabIndex == -1)
            {
                _closeButtonHovered = false;
                return;
            }

            TabLayout hoveredTabLayout = layoutCalculator.GetTabLayout(_activeTabs, _hoveredTabIndex, _displayedTabIndex);
            _closeButtonHovered = hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition);
        }

        public void MouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (_hoveredTab == null)
            {
                return;
            }

            TabLayout hoveredTabLayout = layoutCalculator.GetTabLayout(_activeTabs, _hoveredTabIndex, _displayedTabIndex);
            
            if (e.FirstTimeButtons.HasLeft)
            {
                if (hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition))
                {
                    _closeButtonHeld = true;
                }
                else if (hoveredTabLayout.BoxBounds.Contains(e.MousePosition) && _hoveredTab != _displayedTab)
                {
                    _displayedTab = _hoveredTab;
                    _displayedTabIndex = _hoveredTabIndex;
                    _closeButtonHovered = hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition);

                    DisplayedTabChanged?.Invoke(_hoveredTab);
                }
            }
            else if (e.ReleasedButtons.HasLeft)
            {
                if (_closeButtonHeld && hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition))
                {
                    _closeButtonHeld = false;
                    _activeTabs.Remove(_hoveredTab);
                    MousePositionChangeUpdate(e); // update hovered tab for new layout
                }
            }
        }

        private (Tab? HoveredTab, int HoveredTabIndex) FindHoveredTab(Point mousePosition)
        {
            for (int i = 0; i < _activeTabs.Count; i++)
            {
                TabLayout layout = layoutCalculator.GetTabLayout(_activeTabs, i, _displayedTabIndex);

                if (layout.BoxBounds.Contains(mousePosition))
                {
                    return (_activeTabs[i], i);
                }
            }

            return (null, -1);
        }
    }
}
