using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;

#nullable enable

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabListButton
    {
        private readonly TabsHeaderLayoutCalculator _layoutCalculator;
        private readonly TabCollection _activeTabs;

        private Tab? _hoveredTabOption;

        // close button of hovered tab option
        private bool _closeButtonHovered;
        private bool _closeButtonHeld;
        private bool _tabHeld;

        public bool CloseButtonHovered => _closeButtonHovered;
        public bool CloseButtonHeld => _closeButtonHeld;
        public bool HoveredTabIsHeld => _tabHeld;
        public Tab? HoveredTab => _hoveredTabOption;

        public TabListButton(TabCollection activeTabs, TabsHeaderLayoutCalculator layoutCalculator)
        {
            _layoutCalculator = layoutCalculator;
            _activeTabs = activeTabs;
        }

        public void HoverUpdate(Point mousePosition, int displayedTabIndex)
        {
             _hoveredTabOption = TryHitOption(mousePosition, displayedTabIndex);

            if (_hoveredTabOption == null)
            {
                _closeButtonHovered = false;
                return;
            }

            TabLayout hoveredTabLayout = _layoutCalculator.GetTabListOptionLayout(_layoutCalculator.GetTabListMenuBounds(displayedTabIndex), 
                                                                                  _activeTabs.IndexOf(_hoveredTabOption), displayedTabIndex);
            _closeButtonHovered = hoveredTabLayout.CloseButtonBounds.Contains(mousePosition);
        }

        public void HoldStart(Point mousePosition, int displayedTabIndex)
        {
            if (_hoveredTabOption != null)
            {
                TabLayout optionLayout = _layoutCalculator.GetTabListOptionLayout(_layoutCalculator.GetTabListMenuBounds(displayedTabIndex),
                                                                                  _activeTabs.IndexOf(_hoveredTabOption), displayedTabIndex);

                if (optionLayout.CloseButtonBounds.Contains(mousePosition))
                {
                    _closeButtonHeld = true;
                }
                else if (optionLayout.BoxBounds.Contains(mousePosition))
                {
                    _tabHeld = true;
                }
            }
        }

        public (Tab? SelectedTab, bool IsCloseButtonSelected) HoldEnded(Point mousePosition, int displayedTabIndex)
        {
            _tabHeld = false;
            _closeButtonHeld = false;

            if (_hoveredTabOption != null)
            {
                TabLayout hoveredOptionLayout = _layoutCalculator.GetTabListOptionLayout(_layoutCalculator.GetTabListMenuBounds(displayedTabIndex),
                                                                                         _activeTabs.IndexOf(_hoveredTabOption), displayedTabIndex);
                if (hoveredOptionLayout.CloseButtonBounds.Contains(mousePosition))
                {
                    return (_hoveredTabOption, true);
                }
                else if (hoveredOptionLayout.BoxBounds.Contains(mousePosition))
                {
                    return (_hoveredTabOption, false);
                }
            }

            return (null, false);
        }

        private Tab? TryHitOption(Point mousePosition, int displayedTabIndex)
        {
            Rectangle tabListMenuBounds = _layoutCalculator.GetTabListMenuBounds(displayedTabIndex);
            for (int i = 0; i < _activeTabs.Count; i++)
            {
                TabLayout optionLayout = _layoutCalculator.GetTabListOptionLayout(tabListMenuBounds, i, displayedTabIndex);

                if (optionLayout.BoxBounds.Contains(mousePosition))
                {
                    return _activeTabs[i];
                }
            }

            return null;
        }
    }
}
