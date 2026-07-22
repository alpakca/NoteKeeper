using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenuRenderer
    {
        private readonly Point iconSize = new Point();
        private readonly Point boundaryPadding = new Point();

        private readonly IEnumerable<SubMenuEntry> openMenuEntries;
        private readonly MenuItemStyleProvider styleProvider;

        public ContextMenuRenderer(IEnumerable<SubMenuEntry> openMenus, MenuItemStyleProvider styleProvider)
        {
            this.openMenuEntries = openMenus;
            this.styleProvider = styleProvider;
        }

        public void Render(RenderTarget2D windowTarget, IContextMenuItem hoveredItem, bool isHeld)
        {
            DrawHelper.SetBeginBatch(windowTarget);

            foreach (var entry in openMenuEntries)
            {
                entry.ContextMenu.RenderBackground();

                Point eachButtonSize = entry.ContextMenu.Boundaries.Size;
                foreach (var item in entry.ContextMenu.Items)
                {
                    MenuItemStyle style = styleProvider.GetStyle(item);
                    Rectangle boundaries = new Rectangle(item.Location, eachButtonSize);

                    DrawHelper.Batch.Draw(style.Background, boundaries, Color.White);

                    if (style.LeftIcon != null)
                    {
                        DrawHelper.Batch.Draw(style.LeftIcon, new Rectangle(boundaries.Location + boundaryPadding, iconSize), Color.White);
                    }

                    DrawHelper.Batch.DrawString(style.LeftTextFont, style.LeftText, new Vector2(boundaries.X + boundaryPadding.X, boundaries.Y + boundaryPadding.Y), Color.White);

                    if (item is IHasRightIcon rightIcon)
                        RenderRightIcon(rightIcon, boundaries);
                    else if (item is IHasRightText rightText)
                        RenderRightText(rightText, boundaries);

                    if (item == hoveredItem)
                    {
                        if (isHeld && style.HeldOverlay != null)
                        {
                            DrawHelper.Batch.Draw(style.HeldOverlay, boundaries, Color.White);
                        }
                        else if (!isHeld && style.HoverOverlay != null)
                        {
                            DrawHelper.Batch.Draw(style.HoverOverlay, boundaries, Color.White);
                        }
                    }
                }
            }

            DrawHelper.EndBatch();
        }

        private void RenderRightIcon(IHasRightIcon rightIcon, Rectangle boundaries)
        {
            if (rightIcon.RightIcon == null)
                return;

            DrawHelper.Batch.Draw(rightIcon.RightIcon, new Rectangle(boundaries.X - boundaryPadding.X - iconSize.X, boundaries.Y + boundaryPadding.Y, 
                                                                     iconSize.X, iconSize.Y), Color.White);
        }

        private void RenderRightText(IHasRightText rightText, Rectangle boundaries)
        {
            if (string.IsNullOrEmpty(rightText.RightText) || rightText.RightTextFont == null)
                return;

            int textWidth = (int)rightText.RightTextFont.MeasureString(rightText.RightText).X;
            DrawHelper.Batch.DrawString(rightText.RightTextFont, rightText.RightText, new Vector2(boundaries.Right - boundaryPadding.X - textWidth,
                                                                                                  boundaries.Y + boundaryPadding.Y),
                                                                                       Color.White);
        }
    }
}
