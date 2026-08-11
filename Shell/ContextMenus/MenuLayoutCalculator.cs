using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class MenuLayoutCalculator
    {
        private const int ContextMenuFrameThickness = 2;

        private readonly MenuItemStyleProvider styleProvider;

        public MenuLayoutCalculator(MenuItemStyleProvider styleProvider)
        {
            this.styleProvider = styleProvider;
        }

        public void CalculateContextMenuSize(ContextMenu contextMenu)
        {
            int maxWidth = 0;
            int totalHeight = 0;

            foreach (var item in contextMenu.Entries)
            {
                Point itemSize = MeasureEntry(item);

                maxWidth = Math.Max(maxWidth, itemSize.X);
                totalHeight += itemSize.Y;
            }

            contextMenu.Size = new Point(maxWidth, totalHeight);
        }

        public Rectangle GetContextMenuBackgroundBounds(ContextMenu menu)
        {
            return new Rectangle(menu.Location.X - ContextMenuFrameThickness, menu.Location.Y - ContextMenuFrameThickness,
                                 menu.Size.X + ContextMenuFrameThickness * 2, menu.Size.Y + ContextMenuFrameThickness * 2);
        }

        public void PositionContextMenuItems(ContextMenu contextMenu)
        {
            int currentY = 0;

            foreach (var item in contextMenu.Entries)
            {
                Point itemSize = MeasureEntry(item);

                item.Location = contextMenu.Location + new Point(0, currentY);
                item.Size = new Point(contextMenu.Size.X, itemSize.Y);

                currentY += itemSize.Y;
            }
        }

        private Point MeasureEntry(IContextMenuEntry entry)
        {
            return entry switch
            {
                ActionContextMenuButton action => GetActionButtonSize(action),
                SubContextMenuButton subContext => GetSubContextButtonSize(subContext),
                GroupLineItem line => GetGroupLineSize(line),
                _ => throw new ArgumentException("Unhandled IContextMenuEntry type: " + entry.GetType())
            };
        }

        private Point GetActionButtonSize(ActionContextMenuButton button)
        {
            ContextMenuStyle menuStyle = styleProvider.Style;
            ActionMenuItemStyle itemStyle = (ActionMenuItemStyle)styleProvider.GetStyle(button);

            int leftPartLength = itemStyle.ButtonStyle.HorizontalPadding + itemStyle.ButtonStyle.LeftIconSize.X + itemStyle.ButtonStyle.IconHorizontalPadding + (int)menuStyle.Font.MeasureString(button.LeftText).X;
            int rightPartLength = itemStyle.ButtonStyle.HorizontalPadding;
            if (button.RightText != null)
            {
                rightPartLength += (int)menuStyle.Font.MeasureString(button.RightText).X;
            }

            return new Point(leftPartLength + itemStyle.ButtonStyle.LeftRightPartPadding + rightPartLength, itemStyle.Height);
        }

        private Point GetSubContextButtonSize(SubContextMenuButton button)
        {
            ContextMenuStyle menuStyle = styleProvider.Style;
            SubContextMenuItemStyle itemStyle = (SubContextMenuItemStyle)styleProvider.GetStyle(button);

            int leftPartLength = itemStyle.ButtonStyle.HorizontalPadding + itemStyle.ButtonStyle.LeftIconSize.X + itemStyle.ButtonStyle.IconHorizontalPadding + (int)menuStyle.Font.MeasureString(button.LeftText).X;
            int rightPartLength = itemStyle.ButtonStyle.HorizontalPadding + itemStyle.RightIconSize.X;

            return new Point(leftPartLength + itemStyle.ButtonStyle.LeftRightPartPadding + rightPartLength, itemStyle.Height);
        }

        private Point GetGroupLineSize(GroupLineItem line)
        {
            GroupLineStyle style = (GroupLineStyle)styleProvider.GetStyle(line);

            return new Point(0, style.Height);
        }

        public MenuItemLayout GetEntryLayout(IContextMenuEntry entry)
        {
            return entry switch
            {
                ActionContextMenuButton button => GetActionButtonLayout(button),
                SubContextMenuButton button => GetSubContextButtonLayout(button),
                GroupLineItem line => GetGroupLineLayout(line),
                _ => throw new ArgumentException("Unhandled IContextMenuEntry type: " + entry.GetType())
            };
        }

        private MenuItemLayout GetActionButtonLayout(ActionContextMenuButton button)
        {
            ContextMenuStyle menuStyle = styleProvider.Style;
            ActionMenuItemStyle itemStyle = (ActionMenuItemStyle)styleProvider.GetStyle(button);
            Rectangle bounds = new Rectangle(button.Location, button.Size);

            int rightTextWidth = button.RightText != null ? (int)menuStyle.Font.MeasureString(button.RightText).X : 0;
            return new MenuItemLayout
            {
                ItemBounds = bounds,
                LeftIconBounds = new Rectangle(new Point(bounds.X + itemStyle.ButtonStyle.HorizontalPadding, bounds.Y + (bounds.Height - itemStyle.ButtonStyle.LeftIconSize.Y) / 2), 
                                               itemStyle.ButtonStyle.LeftIconSize),

                LeftTextPosition = new Vector2(bounds.X + itemStyle.ButtonStyle.HorizontalPadding + itemStyle.ButtonStyle.LeftIconSize.X + itemStyle.ButtonStyle.IconHorizontalPadding,
                                               bounds.Y + (bounds.Height - menuStyle.Font.LineSpacing) / 2),

                RightTextPosition = new Vector2(bounds.Right - itemStyle.ButtonStyle.HorizontalPadding - rightTextWidth,
                                                bounds.Y + (bounds.Height - menuStyle.Font.LineSpacing) / 2),
            };
        }

        private MenuItemLayout GetSubContextButtonLayout(SubContextMenuButton button)
        {
            ContextMenuStyle menuStyle = styleProvider.Style;
            SubContextMenuItemStyle itemStyle = (SubContextMenuItemStyle)styleProvider.GetStyle(button);
            Rectangle bounds = new Rectangle(button.Location, button.Size);

            return new MenuItemLayout
            {
                ItemBounds = bounds,
                LeftTextPosition = new Vector2(bounds.X + itemStyle.ButtonStyle.HorizontalPadding + itemStyle.ButtonStyle.LeftIconSize.X + itemStyle.ButtonStyle.IconHorizontalPadding,
                                               bounds.Y + (bounds.Height - menuStyle.Font.LineSpacing) / 2),

                RightIconBounds = new Rectangle(bounds.Right - itemStyle.ButtonStyle.HorizontalPadding - itemStyle.RightIconSize.X, 
                                                bounds.Y + (bounds.Height - itemStyle.RightIconSize.Y) / 2,
                                                itemStyle.RightIconSize.X, 
                                                itemStyle.RightIconSize.Y),
            };
        }

        private MenuItemLayout GetGroupLineLayout(GroupLineItem line)
        {
            GroupLineStyle style = (GroupLineStyle)styleProvider.GetStyle(line);

            return new MenuItemLayout
            {
                ItemBounds = new Rectangle(line.Location.X, line.Location.Y + (style.Height - style.Thickness) / 2, line.Size.X, style.Thickness)
            };
        }
    }
}
