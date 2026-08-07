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

        public void Render(TabCollection activeTabs, int displayedTabIndex, int hoveredTabIndex)
        {
            TabsHeaderStyle style = _styleProvider.Style;
            DrawHelper.Batch.Draw(style.HeaderBackground, _layoutCalculator.ScreenBounds, Color.White);
            DrawHelper.Batch.Draw(style.TabListButtonIcon, _layoutCalculator.TabListButtonBounds, Color.White);

            for (int i = 0; i < activeTabs.Count; i++)
            {
                TabLayout layout = _layoutCalculator.GetTabLayout(i, displayedTabIndex);
                if (layout.BoxBounds.Right > _layoutCalculator.ScreenBounds.Right)
                {
                    break;
                }

                Texture2D background = i == displayedTabIndex ? style.DisplayedTabBackground
                                                              : i == hoveredTabIndex ? style.TabHoveredBackground
                                                                                     : style.TabDefaultBackground;

                SpriteFont font = i == displayedTabIndex ? style.DisplayedTabFont : style.DefaultFont;
                Texture2D? closeButtonTexture = i != displayedTabIndex && i != hoveredTabIndex ? null : style.TabCloseButtonIcon;

                DrawTabBox(activeTabs[i], layout, background, font, closeButtonTexture);
            }
        }

        public void DrawHoveredButtonOverlay(Rectangle hoveredButtonBounds, bool isButtonHeld)
        {
            TabsHeaderStyle style = _styleProvider.Style;

            DrawHelper.Batch.Draw(isButtonHeld ? style.ButtonHeldOverlay : style.ButtonHoverOverlay, hoveredButtonBounds, Color.White);
        }

        public void DrawTabList(TabCollection activeTabs, TabListButton button, int displayedTabIndex)
        {
            TabsHeaderStyle style = _styleProvider.Style;
            Rectangle tabListMenuBounds = _layoutCalculator.GetTabListMenuBounds(displayedTabIndex);

            DrawHelper.Batch.Draw(style.HeaderBackground, tabListMenuBounds, Color.White);
            DrawHelper.Batch.Draw(style.ButtonHoverOverlay, _layoutCalculator.TabListButtonBounds, Color.White);

            int hoveredTabIndex = activeTabs.IndexOf(button.HoveredTab!);
            for (int i = 0; i < activeTabs.Count; i++)
            {
                TabLayout optionLayout = _layoutCalculator.GetTabListOptionLayout(tabListMenuBounds, i, displayedTabIndex);

                Texture2D background = i == displayedTabIndex ? style.DisplayedTabBackground
                                              : i == hoveredTabIndex ? style.TabHoveredBackground
                                                                     : style.TabDefaultBackground;

                SpriteFont font = i == displayedTabIndex ? style.DisplayedTabFont : style.DefaultFont;
                Texture2D? closeButtonTexture = i != displayedTabIndex && i != hoveredTabIndex ? null : style.TabCloseButtonIcon;

                DrawTabBox(activeTabs[i], optionLayout, background, font, closeButtonTexture);

                if (i == hoveredTabIndex)
                {
                    DrawHelper.Batch.Draw(button.HoveredTabIsHeld ? style.ButtonHeldOverlay : style.ButtonHoverOverlay, optionLayout.BoxBounds, Color.White);

                    if (button.CloseButtonHovered)
                    {
                        DrawHelper.Batch.Draw(button.CloseButtonHeld ? style.ButtonHeldOverlay : style.ButtonHoverOverlay, optionLayout.CloseButtonBounds, Color.White);
                    }
                }
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
