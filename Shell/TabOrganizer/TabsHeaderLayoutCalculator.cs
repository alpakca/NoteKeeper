using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.Styling;
using System;

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabsHeaderLayoutCalculator
    {
        public const int TextSize = 12;

        private const int BoxHorizontalMargin = 2;
        private int CloseButtonLength => ScreenBounds.Height;

        private readonly TabCollection _activeTabs;

        public Rectangle ScreenBounds { get; set; }
        public Rectangle TabListButtonBounds => new Rectangle(ScreenBounds.Right - CloseButtonLength, ScreenBounds.Y, CloseButtonLength, CloseButtonLength);

        public TabsHeaderLayoutCalculator(TabCollection activeTabs)
        {
            _activeTabs = activeTabs;
        }

        public TabLayout GetTabLayout(int index, int displayedTabIndex)
        {
            if (index == -1)
            {
                return new TabLayout();
            }

            int xoffset = 0;
            for (int i = 0; i < _activeTabs.Count; i++)
            {
                if (i == index)
                {
                    break;
                }

                xoffset += GetWidth(_activeTabs[i], i == displayedTabIndex);
            }

            Rectangle boxBounds = new Rectangle(ScreenBounds.X + xoffset, ScreenBounds.Y, GetWidth(_activeTabs[index], index == displayedTabIndex), CloseButtonLength);
            return new TabLayout
            {
                TextOffset = GetNameOffset(index == displayedTabIndex),
                BoxBounds = boxBounds,
                CloseButtonBounds = new Rectangle(boxBounds.Right - CloseButtonLength, boxBounds.Y, CloseButtonLength, CloseButtonLength)
            };
        }

        public Rectangle GetTabListMenuBounds(int displayedTabIndex)
        {
            int maxWidth = 0;
            for (int i = 0; i < _activeTabs.Count; i++)
            {
                maxWidth = Math.Max(maxWidth, GetWidth(_activeTabs[i], displayedTabIndex == i));
            }

            Point size = new Point(maxWidth, CloseButtonLength);
            Point location = new Point(TabListButtonBounds.Right - maxWidth, TabListButtonBounds.Bottom);

            return new Rectangle(location, size);
        }

        public TabLayout GetTabListOptionLayout(Rectangle tabListMenuBounds, int index, int displayedTabIndex)
        {
            Rectangle box = new Rectangle(tabListMenuBounds.X, tabListMenuBounds.Y + index * CloseButtonLength, tabListMenuBounds.Width, CloseButtonLength);
            return new TabLayout
            {
                TextOffset = GetNameOffset(index == displayedTabIndex),
                BoxBounds = box,
                CloseButtonBounds = new Rectangle(box.Right - CloseButtonLength, box.Y, CloseButtonLength, CloseButtonLength)
            };
        }

        private Point GetNameOffset(bool isDisplayed)
        {
            return new Point(BoxHorizontalMargin, (ScreenBounds.Height - FontProvider.GetLineSpacing(TextSize, isDisplayed)) / 2);
        }

        private int GetWidth(Tab tab, bool isDisplayed)
        {
            int unmodifiedWidth = BoxHorizontalMargin * 2 + (int)FontProvider.MeasureWidth(TextSize, isBold: isDisplayed, tab.Name) + CloseButtonLength;

            return unmodifiedWidth + (tab.TabHistory.IsModified ? (int)FontProvider.MeasureWidth(TextSize, isBold: isDisplayed, "*") : 0);
        }
    }
}
