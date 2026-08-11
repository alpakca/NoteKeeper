using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenuRenderer
    {
        private readonly BatchDrawOptions _drawOptions;

        private readonly IEnumerable<SubMenuPair> openMenuPairs;
        private readonly MenuItemStyleProvider styleProvider;
        private readonly MenuLayoutCalculator layoutCalculator;

        public ContextMenuRenderer(IEnumerable<SubMenuPair> openMenus, MenuItemStyleProvider styleProvider, MenuLayoutCalculator layoutCalculator)
        {
            _drawOptions = new BatchDrawOptions { BlendState = BlendState.AlphaBlend };

            this.openMenuPairs = openMenus;
            this.styleProvider = styleProvider;
            this.layoutCalculator = layoutCalculator;
        }

        public void Render(RenderTarget2D windowTarget, IContextMenuItem hoveredItem, bool isHeld)
        {
            ContextMenuStyle style = styleProvider.Style;

            DrawHelper.SetBeginBatch(windowTarget,  _drawOptions);
            
            foreach (SubMenuPair pair in openMenuPairs)
            {
                DrawTexture(style.Background, layoutCalculator.GetContextMenuBackgroundBounds(pair.ContextMenu));

                foreach (IContextMenuEntry entry in pair.ContextMenu.Entries)
                {
                    switch (entry)
                    {
                        case ActionContextMenuButton button:
                            DrawActionButton(button);
                            break;

                        case SubContextMenuButton button:
                            DrawSubContextMenuButton(button);
                            break;

                        case GroupLineItem line:
                            DrawGroupLine(line);
                            break;

                        default:
                            throw new ArgumentException("Unhandled IContextMenuEntry type: " + entry.GetType());
                    }

                    if (entry == hoveredItem)
                    {
                        DrawTexture(isHeld ? style.HeldOverlay : style.HoverOverlay, new Rectangle(entry.Location, entry.Size));
                    }
                }
            }

            DrawHelper.EndBatch();
        }

        private void DrawActionButton(ActionContextMenuButton button)
        {
            ActionMenuItemStyle style = (ActionMenuItemStyle)styleProvider.GetStyle(button);
            MenuItemLayout layout = layoutCalculator.GetEntryLayout(button);

            if (button.IconTexture != null)
            {
                DrawTexture(button.IconTexture, layout.LeftIconBounds);
            }

            DrawString(button.LeftText, styleProvider.Style.Font, layout.LeftTextPosition, style.ButtonStyle.LeftTextColor);

            if (button.RightText != null)
            {
                DrawString(button.RightText, styleProvider.Style.Font, layout.RightTextPosition, style.RightTextColor);
            }
        }

        private void DrawSubContextMenuButton(SubContextMenuButton button)
        {
            SubContextMenuItemStyle style = (SubContextMenuItemStyle)styleProvider.GetStyle(button);
            MenuItemLayout layout = layoutCalculator.GetEntryLayout(button);

            DrawString(button.LeftText, styleProvider.Style.Font, layout.LeftTextPosition, style.ButtonStyle.LeftTextColor);
            DrawTexture(button.RightIconTexture, layout.RightIconBounds);
        }

        private void DrawGroupLine(GroupLineItem line)
        {
            GroupLineStyle style = (GroupLineStyle)styleProvider.GetStyle(line);
            MenuItemLayout layout = layoutCalculator.GetEntryLayout(line);

            DrawHelper.DrawBackground(layout.ItemBounds, style.LineColor);
        }

        private static void DrawTexture(Texture2D texture, Rectangle destination)
        {
            DrawHelper.Batch.Draw(texture, destination, Color.White);
        }

        private static void DrawString(string text, SpriteFont font, Vector2 position, Color color)
        {
            DrawHelper.Batch.DrawString(font, text, position, color);
        }
    }
}
