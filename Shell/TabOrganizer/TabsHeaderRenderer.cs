using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

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

        public void Render(IList<Tab> activeTabs, Tab? displayedTab, Tab? hoveredTab, bool closeButtonHovered, bool closeButtonHeld)
        {
            TabsHeaderStyle style = _styleProvider.Style;

            DrawHelper.Batch.Draw(style.HeaderBackground, _layoutCalculator.ScreenBounds, Color.White);

            int displayedTabIndex = activeTabs.IndexOf(displayedTab!);

            // there can't be situation no displayed tab exist / is not inside active tabs
            // it means no tabs are active at all, in that case just return
            if (displayedTabIndex == -1)
            {
                return;
            }

            int currentXOffset = 0;
            for (int i = 0; i < activeTabs.Count; i++)
            {
                TabLayout layout = _layoutCalculator.GetTabLayout(activeTabs, i, displayedTabIndex);

                if (currentXOffset + layout.BoxBounds.X > _layoutCalculator.ScreenBounds.Width)
                {
                    break;
                }

                Tab tab = activeTabs[i];
                var textureSet = GetTextureSet(tab == displayedTab, tab == hoveredTab, closeButtonHovered, closeButtonHeld);
                DrawTabBox(tab, layout, textureSet.Background, textureSet.Font, textureSet.CloseButtonTexture);
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

        private (Texture2D Background, SpriteFont Font, Texture2D? CloseButtonTexture) GetTextureSet(bool isTabDisplayed, bool isTabHovered, 
                                                                                                    bool isCloseButtonHovered, bool isCloseButtonHeld)
        {
            TabsHeaderStyle style = _styleProvider.Style;
            if (!isTabDisplayed && !isTabHovered)
            {
                return (style.TabDefaultBackground, style.DefaultFont, null);
            }

            // tab is either hovered or held, close button appears
            Texture2D closeButtonTexture = isCloseButtonHeld ? style.TabCloseButtonHeldBackground 
                                                             : (isCloseButtonHovered ? style.TabCloseButtonHoverBackground 
                                                                                     : style.TabCloseButtonDefaultBackground);
            if (isTabDisplayed)
            {
                return (style.DisplayedTabBackground, style.DisplayedTabFont, closeButtonTexture);
            }
            else if (isTabHovered)
            {
                return (style.TabHoveredBackground, style.DefaultFont, closeButtonTexture);
            }
            else
            {
                return (style.TabDefaultBackground, style.DefaultFont, closeButtonTexture);
            }
        }
    }
}
