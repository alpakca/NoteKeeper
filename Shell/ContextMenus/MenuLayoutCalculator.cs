using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class MenuLayoutCalculator
    {
        private readonly MenuItemStyleProvider styleProvider;

        public MenuLayoutCalculator(MenuItemStyleProvider styleProvider)
        {
            this.styleProvider = styleProvider;
        }

        public void CalculateContextMenuSize(ContextMenu contextMenu)
        {
            int maxWidth = 0;
            int totalHeight = 0;

            foreach (var item in contextMenu.Items)
            {
                Point itemSize = MeasureItem(item);

                maxWidth = Math.Max(maxWidth, itemSize.X);
                totalHeight += itemSize.Y;
            }

            contextMenu.Size = new Point(maxWidth, totalHeight);
        }

        public void PositionContextMenuItems(ContextMenu contextMenu)
        {
            int currentY = 0;

            foreach (var item in contextMenu.Items)
            {
                Point itemSize = MeasureItem(item);

                item.Location = contextMenu.Location + new Point(0, currentY);
                item.Size = new Point(contextMenu.Size.X, itemSize.Y);

                currentY += itemSize.Y;
            }
        }

        private Point MeasureItem(IContextMenuItem item)
        {
            MenuItemStyle style = styleProvider.GetStyle(item);

            int iconYSize = style.BoundaryPadding.Y * 2 + style.IconSize.Y;
            int textYSize = style.BoundaryPadding.Y * 2 + style.LeftTextFont.LineSpacing;

            Point size = new Point(style.BoundaryPadding.X * 2 + style.IconSize.X + style.IconHorizontalPadding + (int)style.LeftTextFont.MeasureString(style.LeftText).X, 
                                   Math.Max(iconYSize, textYSize));

            if (item is IHasRightIcon)
            {
                size.X += style.IconHorizontalPadding * 2 + style.IconSize.X;
            }
            else if (item is IHasRightText rightText && rightText.RightTextFont != null && !string.IsNullOrEmpty(rightText.RightText))
            {
                size.X += rightText.RightTextHorizontalPadding * 2 + (int)style.LeftTextFont.MeasureString(rightText.RightText).X;
            }

            return size;
        }
    }
}
