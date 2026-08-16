using Microsoft.Xna.Framework;
using NoteKeeper.Shell.Styling;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenuRenderer
    {
        private const float RightTextColorAlphaMultiplier = 0.7f;

        private readonly IEnumerable<SubMenuPair> _openMenuPairs;
        private readonly ContextMenuLayoutCalculator _layoutCalculator;

        public ContextMenuRenderer(IEnumerable<SubMenuPair> openMenus, ContextMenuLayoutCalculator layoutCalculator)
        {
            _openMenuPairs = openMenus;
            _layoutCalculator = layoutCalculator;
        }

        public void Render(IContextMenuItem hoveredItem, bool isHeld)
        {
            ResolvedTheme theme = ThemeService.CurrentTheme;
            foreach (SubMenuPair pair in _openMenuPairs)
            {
                DrawHelper.DrawBorderedBackground(_layoutCalculator.GetContextMenuBackgroundBounds(pair.ContextMenu),
                                                  theme.GetColor(ContextMenuStyles.Border), theme.GetColor(ContextMenuStyles.Background), 1);

                foreach (IContextMenuEntry entry in pair.ContextMenu.Entries)
                {
                    switch (entry)
                    {
                        case ActionContextMenuButton button:
                            DrawActionButton(button, theme);
                            break;

                        case SubContextMenuButton button:
                            DrawSubContextMenuButton(button, theme);
                            break;

                        case GroupLineItem line:
                            DrawGroupLine(line, theme);
                            break;

                        default:
                            throw new ArgumentException("Unhandled IContextMenuEntry type: " + entry.GetType());
                    }

                    if (entry == hoveredItem)
                    {
                        DrawHelper.DrawBackground(new Rectangle(entry.Location, entry.Size), 
                                                  isHeld ? theme.GetColor(ContextMenuStyles.HeldOverlay) : theme.GetColor(ContextMenuStyles.HoverOverlay));
                    }
                }
            }
        }

        private void DrawActionButton(ActionContextMenuButton button, ResolvedTheme theme)
        {
            MenuItemLayout layout = _layoutCalculator.GetEntryLayout(button);

            if (button.IconTexture != null)
            {
                DrawHelper.Batch.Draw(button.IconTexture, layout.LeftTextPosition, Color.White);
            }

            FontProvider.DrawText(ContextMenuLayoutCalculator.TextSize, isBold: false, button.LeftText, 
                                  layout.LeftTextPosition, theme.GetColor(ContextMenuStyles.TextColor));

            if (button.RightText != null)
            {
                FontProvider.DrawText(ContextMenuLayoutCalculator.TextSize, isBold: false, button.RightText,
                                      layout.RightTextPosition, theme.GetColor(ContextMenuStyles.TextColor) * RightTextColorAlphaMultiplier);
            }
        }

        private void DrawSubContextMenuButton(SubContextMenuButton button, ResolvedTheme theme)
        {
            MenuItemLayout layout = _layoutCalculator.GetEntryLayout(button);

            FontProvider.DrawText(ContextMenuLayoutCalculator.TextSize, isBold: false, button.LeftText, 
                                  layout.LeftTextPosition, theme.GetColor(ContextMenuStyles.TextColor));

            DrawHelper.Batch.Draw(theme.GetIcon(ContextMenuStyles.SubMenuButtonRightIcon), layout.RightIconBounds, Color.White);
        }

        private void DrawGroupLine(GroupLineItem line, ResolvedTheme theme)
        {
            MenuItemLayout layout = _layoutCalculator.GetEntryLayout(line);

            DrawHelper.DrawBackground(layout.ItemBounds, theme.GetColor(ContextMenuStyles.GroupLine));
        }
    }
}
