using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

#nullable enable

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabsHeaderRenderer
    {
        private readonly TabsHeaderStyleProvider _styleProvider;
        private readonly TabsHeaderLayoutCalculator _layoutCalculator;

        public TabsHeaderRenderer(TabsHeaderStyleProvider styleProvider, TabsHeaderLayoutCalculator layoutCalculator)
        {
            _styleProvider = styleProvider;
            _layoutCalculator = layoutCalculator;
        }

        public void Render(IList<Tab> activeTabs, int displayedTabIndex, int hoveredTabIndex, bool closeButtonHovered, bool closeButtonHeld)
        {
            TabsHeaderStyle style = _styleProvider.Style;
            DrawHelper.Batch.Draw(style.HeaderBackground, _layoutCalculator.ScreenBounds, Color.White);

            int currentXOffset = 0;
            for (int i = 0; i < activeTabs.Count; i++)
            {
                TabLayout layout = _layoutCalculator.GetTabLayout(activeTabs, i, displayedTabIndex);
                if (currentXOffset + layout.BoxBounds.X > _layoutCalculator.ScreenBounds.Width)
                {
                    break;
                }

                Texture2D? closeButtonTexture;
                if (i != displayedTabIndex && i != hoveredTabIndex)
                {
                    closeButtonTexture = null;
                }
                else
                {
                    closeButtonTexture = closeButtonHeld ? style.TabCloseButtonHeldBackground
                                                         : closeButtonHovered ? style.TabCloseButtonHoverBackground
                                                                              : style.TabCloseButtonDefaultBackground;
                    if (i != hoveredTabIndex)
                    {
                        closeButtonTexture = style.TabCloseButtonDefaultBackground;
                    }
                }

                Texture2D background = i == displayedTabIndex ? style.DisplayedTabBackground
                                                              : i == hoveredTabIndex ? style.TabHoveredBackground
                                                                                     : style.TabDefaultBackground;

                SpriteFont font = i == displayedTabIndex ? style.DisplayedTabFont : style.DefaultFont;

                DrawTabBox(activeTabs[i], layout, background, font, closeButtonTexture);
            }
        }

        private static void DrawTabBox(Tab tab, TabLayout layout, Texture2D background, SpriteFont font, Texture2D? closeButtonTexture)
        {
            DrawHelper.Batch.Draw(background, layout.BoxBounds, Color.White);
            DrawHelper.Batch.DrawString(font, (tab.TabSerializer.IsModified ? tab.Name + "*" : tab.Name), 
                                        (layout.BoxBounds.Location + layout.TextOffset).ToVector2(), Color.White);

            if (closeButtonTexture != null)
            {
                DrawHelper.Batch.Draw(closeButtonTexture, layout.CloseButtonBounds, Color.White);
            }
        }
    }
}
