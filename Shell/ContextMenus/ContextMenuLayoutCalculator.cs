using Microsoft.Xna.Framework;
using NoteKeeper.Shell.Styling;
using System;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenuLayoutCalculator
    {
        private const int ContextMenuFrameThickness = 2;

        private const int BoxHorizontalPadding = 2;
        private const int IconHorizontalPadding = 2;
        private const int IconWidth = 25;
        private const int IconHeight = 25;
        private const int LeftRightPartPadding = 20;

        public const int TextSize = 12;
        public const int ButtonHeight = 30;

        public const int GroupLineHeight = 4;
        public const int GroupLineThickness = 2;

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
                GroupLineItem line => new Point(0, GroupLineHeight),
                _ => throw new ArgumentException("Unhandled IContextMenuEntry type: " + entry.GetType())
            };
        }

        private Point GetActionButtonSize(ActionContextMenuButton button)
        {
            int leftPartLength = BoxHorizontalPadding + IconWidth + IconHorizontalPadding + (int)FontProvider.MeasureWidth(TextSize, isBold: false, button.LeftText);
            int rightPartLength = BoxHorizontalPadding;
            if (button.RightText != null)
            {
                rightPartLength += (int)FontProvider.MeasureWidth(TextSize, isBold: false, button.RightText);
            }

            return new Point(leftPartLength + LeftRightPartPadding + rightPartLength, ButtonHeight);
        }

        private Point GetSubContextButtonSize(SubContextMenuButton button)
        {
            int leftPartLength = BoxHorizontalPadding + IconWidth + IconHorizontalPadding + (int)FontProvider.MeasureWidth(TextSize, isBold: false, button.LeftText);
            int rightPartLength = BoxHorizontalPadding + IconWidth;

            return new Point(leftPartLength + LeftRightPartPadding + rightPartLength, ButtonHeight);
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
            Rectangle bounds = new Rectangle(button.Location, button.Size);

            int rightTextWidth = button.RightText == null ? 0 : (int)FontProvider.MeasureWidth(TextSize, isBold: false, button.RightText);
            return new MenuItemLayout
            {
                ItemBounds = bounds,
                LeftIconBounds = new Rectangle(bounds.X + BoxHorizontalPadding, bounds.Y + (bounds.Height - IconHeight) / 2, 
                                               IconWidth, IconHeight),

                LeftTextPosition = new Vector2(bounds.X + BoxHorizontalPadding + IconWidth + IconHorizontalPadding,
                                               bounds.Y + (bounds.Height - FontProvider.GetLineSpacing(TextSize, isBold: false)) / 2),

                RightTextPosition = new Vector2(bounds.Right - BoxHorizontalPadding - rightTextWidth,
                                                bounds.Y + (bounds.Height - FontProvider.GetLineSpacing(TextSize, isBold: false)) / 2),
            };
        }

        private MenuItemLayout GetSubContextButtonLayout(SubContextMenuButton button)
        {
            Rectangle bounds = new Rectangle(button.Location, button.Size);

            return new MenuItemLayout
            {
                ItemBounds = bounds,
                LeftTextPosition = new Vector2(bounds.X + BoxHorizontalPadding + IconWidth +IconHorizontalPadding,
                                               bounds.Y + (bounds.Height - FontProvider.GetLineSpacing(TextSize, isBold: false)) / 2),

                RightIconBounds = new Rectangle(bounds.Right - BoxHorizontalPadding - IconWidth, bounds.Y + (bounds.Height - IconHeight) / 2,
                                                IconWidth, IconHeight),
            };
        }

        private MenuItemLayout GetGroupLineLayout(GroupLineItem line)
        {
            return new MenuItemLayout
            {
                ItemBounds = new Rectangle(line.Location.X, line.Location.Y + (GroupLineHeight -GroupLineThickness) / 2, line.Size.X, GroupLineThickness)
            };
        }
    }
}
