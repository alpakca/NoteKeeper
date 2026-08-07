using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabsHeaderLayoutCalculator
    {
        private const int BoxHorizontalMargin = 2;
        private int CloseButtonLength => ScreenBounds.Height;

        private readonly TabsHeaderStyleProvider _styleProvider;
        private readonly TabCollection _activeTabs;

        public Rectangle ScreenBounds { get; set; }
        public Rectangle TabListButtonBounds => new Rectangle(ScreenBounds.Right - CloseButtonLength, ScreenBounds.Y, CloseButtonLength, CloseButtonLength);

        public TabsHeaderLayoutCalculator(TabsHeaderStyleProvider styleProvider, TabCollection activeTabs)
        {
            _styleProvider = styleProvider;
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
            int lineSpacing = isDisplayed ? _styleProvider.Style.DisplayedTabFont.LineSpacing : _styleProvider.Style.DefaultFont.LineSpacing;
            return new Point(BoxHorizontalMargin, (ScreenBounds.Height - lineSpacing) / 2);
        }

        private int GetWidth(Tab tab, bool isDisplayed)
        {
            TabsHeaderStyle style = _styleProvider.Style;
            SpriteFont fontToUse = isDisplayed ? style.DisplayedTabFont : style.DefaultFont;

            return (int)(BoxHorizontalMargin * 2 + fontToUse.MeasureString(tab.Name).X + (tab.TabSerializer.IsModified ? fontToUse.MeasureString("*").X : 0)
                       + CloseButtonLength);
        }
    }
}
