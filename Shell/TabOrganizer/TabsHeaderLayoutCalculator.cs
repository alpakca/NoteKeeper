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

        public Rectangle ScreenBounds { get; set; }

        public TabsHeaderLayoutCalculator(TabsHeaderStyleProvider styleProvider)
        {
            _styleProvider = styleProvider;
        }

        public TabLayout GetTabLayout(TabCollection activeTabs, int index, int displayedTabIndex)
        {
            if (index == -1)
            {
                return new TabLayout();
            }

            int xoffset = 0;
            for (int i = 0; i < activeTabs.Count; i++)
            {
                if (i == index)
                {
                    break;
                }

                xoffset += GetWidth(activeTabs[i], i == displayedTabIndex);
            }

            Rectangle boxBounds = new Rectangle(ScreenBounds.X + xoffset, ScreenBounds.Y, GetWidth(activeTabs[index], index == displayedTabIndex), CloseButtonLength);
            return new TabLayout
            {
                TextOffset = GetNameOffset(index == displayedTabIndex),
                BoxBounds = boxBounds,
                CloseButtonBounds = new Rectangle(boxBounds.Right - CloseButtonLength, boxBounds.Y, CloseButtonLength, CloseButtonLength)
            };
        }

        private Point GetNameOffset(bool isDisplayed)
        {
            int lineSpacing = isDisplayed ? _styleProvider.Style.DisplayedTabFont.LineSpacing : _styleProvider.Style.DefaultFont.LineSpacing;
            return new Point(BoxHorizontalMargin, ScreenBounds.Height - lineSpacing);
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
