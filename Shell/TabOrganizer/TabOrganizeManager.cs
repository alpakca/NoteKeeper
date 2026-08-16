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
        private readonly TabListButton tabList;

        private readonly TabCollection _activeTabs;
        private Tab? _displayedTab;
        private Tab? _hoveredTab;

        private State _state;
        private Rectangle _switchedTabsUnion;

        private Rectangle _hoveredButtonBounds;

        public Rectangle ScreenBounds
        {
            get => layoutCalculator.ScreenBounds;
            set => layoutCalculator.ScreenBounds = value;
        }

        public event Action<Tab>? DisplayedTabChanged;

        private enum State
        {
            Idle,
            SwitchingTabs,
            SelectingButton,
            TabListOpen
        }

        public TabOrganizeManager(TabCollection activeTabs)
        {
            _activeTabs = activeTabs;

            layoutCalculator = new TabsHeaderLayoutCalculator(activeTabs);
            renderer = new TabsHeaderRenderer(layoutCalculator);
            tabList = new TabListButton(activeTabs, layoutCalculator);
        }

        public void DisplayedTabUpdated(Tab? newTab)
        {
            _displayedTab = newTab;
        }

        public void Render()
        {
            renderer.Render(_activeTabs, _activeTabs.IndexOf(_displayedTab!), _activeTabs.IndexOf(_hoveredTab!));

            if (_state == State.Idle)
            {
                renderer.DrawHoveredButtonOverlay(_hoveredButtonBounds, false);
            }
            else if (_state == State.SelectingButton)
            {
                renderer.DrawHoveredButtonOverlay(_hoveredButtonBounds, true);
            }
            else if (_state == State.TabListOpen)
            {
                renderer.DrawTabList(_activeTabs, tabList, _activeTabs.IndexOf(_displayedTab!));
            }
        }

        public void MousePositionChangeUpdate(PopWindowMouseEventArgs e)
        {
            if (_state == State.Idle)
            {
                UpdateHoveredTab(e.MousePosition);
            }
            else if (_state == State.SwitchingTabs)
            {
                HandleTabSwitching(e.MousePosition);
            }
            else if (_state == State.TabListOpen)
            {
                tabList.HoverUpdate(e.MousePosition, _activeTabs.IndexOf(_displayedTab!));
            }
        }

        public void MouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            int hoveredTabIndex = _activeTabs.IndexOf(_hoveredTab!);
            int displayedTabIndex = _activeTabs.IndexOf(_displayedTab!);
            TabLayout hoveredTabLayout = layoutCalculator.GetTabLayout(hoveredTabIndex, displayedTabIndex);

            if (_state == State.Idle)
            {
                if (e.FirstTimeButtons.HasLeft)
                {
                    if (hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition) || layoutCalculator.TabListButtonBounds.Contains(e.MousePosition))
                    {
                        _state = State.SelectingButton;
                    }
                    else if (hoveredTabIndex == displayedTabIndex)
                    {
                        _state = State.SwitchingTabs;
                    }
                    else if (hoveredTabLayout.BoxBounds.Contains(e.MousePosition))
                    {
                        DisplayedTabChanged?.Invoke(_hoveredTab!);
                        UpdateHoveredTab(e.MousePosition);
                    }
                }
            }
            else if (_state == State.SelectingButton)
            {
                if (e.ReleasedButtons.HasLeft)
                {
                    if (hoveredTabLayout.CloseButtonBounds.Contains(e.MousePosition))
                    {
                        _activeTabs.RemoveWithAskingSave(_hoveredTab!);
                        UpdateHoveredTab(e.MousePosition);

                        _state = State.Idle;

                    }
                    else if (layoutCalculator.TabListButtonBounds.Contains(e.MousePosition))
                    {
                        _state = State.TabListOpen;
                    }
                    else
                    {
                        _state = State.Idle;
                    }
                }
            }
            else if (_state == State.TabListOpen)
            {
                if (e.FirstTimeButtons.HasLeft)
                {
                    tabList.HoldStart(e.MousePosition, displayedTabIndex);
                }
                else if (e.ReleasedButtons.HasLeft)
                {
                    (Tab? selectedTab, bool closeButtonSelected) = tabList.HoldEnded(e.MousePosition, displayedTabIndex);
                    if (selectedTab != null)
                    {
                        if (closeButtonSelected)
                            _activeTabs.RemoveWithAskingSave(selectedTab);
                        else
                            DisplayedTabChanged?.Invoke(selectedTab);
                    }

                    UpdateHoveredTab(e.MousePosition);
                    _state = State.Idle; // close tab list whether it clicked a button or not
                }
            }
            else if (_state == State.SwitchingTabs)
            {
                _state = State.Idle;
                ResetSwitchRect();
            }
        }

        private void UpdateHoveredTab(Point mousePosition)
        {
            _hoveredTab = FindHoveredTab(mousePosition);

            if (_hoveredTab != null)
            {
                TabLayout hoveredTabLayout = layoutCalculator.GetTabLayout(_activeTabs.IndexOf(_hoveredTab), _activeTabs.IndexOf(_displayedTab!));
                if (hoveredTabLayout.CloseButtonBounds.Contains(mousePosition))
                {
                    _hoveredButtonBounds = hoveredTabLayout.CloseButtonBounds;
                    return;
                }
            }
            
            if (layoutCalculator.TabListButtonBounds.Contains(mousePosition))
            {
                _hoveredButtonBounds = layoutCalculator.TabListButtonBounds;
            }
            else
            {
                _hoveredButtonBounds = Rectangle.Empty;
            }
        }

        private void HandleTabSwitching(Point mousePosition)
        {
            Tab? hoveredTab = FindHoveredTab(mousePosition);
            if (_displayedTab == null || hoveredTab == null || hoveredTab == _displayedTab)
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
                int hoveredTabIndex = _activeTabs.IndexOf(hoveredTab);
                int displayedTabIndex = _activeTabs.IndexOf(_displayedTab);

                Rectangle hoveredTabBox = layoutCalculator.GetTabLayout(hoveredTabIndex, displayedTabIndex).BoxBounds;
                Rectangle displayedTabBox = layoutCalculator.GetTabLayout(displayedTabIndex, displayedTabIndex).BoxBounds;

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
                TabLayout layout = layoutCalculator.GetTabLayout(i, displayedTabIndex);

                if (layout.BoxBounds.Contains(mousePosition))
                {
                    return _activeTabs[i];
                }
            }

            return null;
        }
    }
}
