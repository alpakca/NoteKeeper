using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System.Collections.Generic;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenuRenderer
    {
        private readonly IEnumerable<SubMenuEntry> openMenuEntries;
        private readonly MenuItemStyleProvider styleProvider;
        private readonly MenuLayoutCalculator layoutCalculator;

        public ContextMenuRenderer(IEnumerable<SubMenuEntry> openMenus, MenuItemStyleProvider styleProvider, MenuLayoutCalculator layoutCalculator)
        {
            this.openMenuEntries = openMenus;
            this.styleProvider = styleProvider;
            this.layoutCalculator = layoutCalculator;
        }

        public void Render(RenderTarget2D windowTarget, IContextMenuItem hoveredItem, bool isHeld)
        {
            DrawHelper.SetBeginBatch(windowTarget);

            foreach (var entry in openMenuEntries)
            {
                entry.ContextMenu.RenderBackground();

                foreach (var item in entry.ContextMenu.Items)
                {
                    MenuItemStyle style = styleProvider.GetStyle(item);
                    MenuItemLayout layout = layoutCalculator.GetItemLayout(item);

                    DrawTexture(style.Background, layout.ItemBounds);

                    if (style.LeftIcon != null)
                        DrawTexture(style.LeftIcon, layout.LeftIconBounds);

                    DrawString(style.LeftText, style.LeftTextFont, layout.LeftTextPosition);

                    if (item is IHasRightIcon rightIcon)
                        DrawTexture(rightIcon.RightIcon, layout.RightIconBounds);

                    else if (item is IHasRightText rightText)
                        DrawString(rightText.RightText, rightText.RightTextFont, layout.RightTextPosition);

                    if (item == hoveredItem)
                    {
                        if (isHeld && style.HeldOverlay != null)
                        {
                            DrawTexture(style.HeldOverlay, layout.ItemBounds);
                        }
                        else if (!isHeld && style.HoverOverlay != null)
                        {
                            DrawTexture(style.HoverOverlay, layout.ItemBounds);
                        }
                    }
                }
            }

            DrawHelper.EndBatch();
        }

        private static void DrawTexture(Texture2D texture, Rectangle destination)
        {
            DrawHelper.Batch.Draw(texture, destination, Color.White);
        }

        private static void DrawString(string text, SpriteFont font, Vector2 position)
        {
            DrawHelper.Batch.DrawString(font, text, position, Color.White);
        }
    }
}
