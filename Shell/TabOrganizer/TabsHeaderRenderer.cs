using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.Styling;
using PopUpUI.Toolkit;

#nullable enable

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabsHeaderRenderer
    {
        private readonly TabsHeaderLayoutCalculator _layoutCalculator;

        public TabsHeaderRenderer(TabsHeaderLayoutCalculator layoutCalculator)
        {
            _layoutCalculator = layoutCalculator;
        }

        public void Render(TabCollection activeTabs, int displayedTabIndex, int hoveredTabIndex)
        {
            ResolvedTheme theme = ThemeService.CurrentTheme;

            DrawHelper.DrawBackground(_layoutCalculator.ScreenBounds, theme.GetColor(TabsHeaderStyles.Surface));
            DrawHelper.Batch.Draw(theme.GetIcon(TabsHeaderStyles.TabListButton), _layoutCalculator.TabListButtonBounds, Color.White);

            for (int i = 0; i < activeTabs.Count; i++)
            {
                TabLayout layout = _layoutCalculator.GetTabLayout(i, displayedTabIndex);
                if (layout.BoxBounds.Right > _layoutCalculator.ScreenBounds.Right)
                {
                    break;
                }

                if (i == displayedTabIndex)
                {
                    DrawDisplayedTabBox(activeTabs[i], layout, theme);
                }
                else
                {
                    DrawDefaultTabBox(activeTabs[i], layout, theme, isHovered: i == hoveredTabIndex);
                }
            }
        }

        public void DrawHoveredButtonOverlay(Rectangle hoveredButtonBounds, bool isButtonHeld)
        {
            DrawHelper.DrawBackground(hoveredButtonBounds, ThemeService.CurrentTheme.GetColor(isButtonHeld ? TabsHeaderStyles.HeldOverlay
                                                                                                           : TabsHeaderStyles.HoverOverlay));
        }

        public void DrawTabList(TabCollection activeTabs, TabListButton button, int displayedTabIndex)
        {
            ResolvedTheme theme = ThemeService.CurrentTheme;
            Rectangle tabListMenuBounds = _layoutCalculator.GetTabListMenuBounds(displayedTabIndex);

            DrawHelper.DrawBackground(_layoutCalculator.ScreenBounds, theme.GetColor(TabsHeaderStyles.Surface));
            DrawHelper.DrawBackground(_layoutCalculator.TabListButtonBounds, theme.GetColor(TabsHeaderStyles.HoverOverlay));

            int hoveredTabIndex = activeTabs.IndexOf(button.HoveredTab!);
            for (int i = 0; i < activeTabs.Count; i++)
            {
                TabLayout optionLayout = _layoutCalculator.GetTabListOptionLayout(tabListMenuBounds, i, displayedTabIndex);

                if (i == displayedTabIndex)
                {
                    DrawDisplayedTabBox(activeTabs[i], optionLayout, theme);
                }
                else
                {
                    DrawDefaultTabBox(activeTabs[i], optionLayout, theme, i == hoveredTabIndex);
                }

                if (i == hoveredTabIndex)
                {
                    DrawHoveredButtonOverlay(optionLayout.BoxBounds, button.HoveredTabIsHeld);

                    if (button.CloseButtonHovered)
                    {
                        DrawHoveredButtonOverlay(optionLayout.CloseButtonBounds, button.CloseButtonHeld);
                    }
                }
            }
        }

        private static void DrawDisplayedTabBox(Tab tab, TabLayout layout, ResolvedTheme theme)
        {
            FontProvider.DrawText(TabsHeaderLayoutCalculator.TextSize, isBold: true, 
                                  (tab.TabHistory.IsModified ? tab.Name + "*" : tab.Name),
                                  (layout.BoxBounds.Location + layout.TextOffset).ToVector2(), 
                                  theme.GetColor(TabsHeaderStyles.TextColor));

            DrawHelper.Batch.Draw(theme.GetIcon(TabsHeaderStyles.CloseButton), layout.CloseButtonBounds, Color.White);
        }

        private static void DrawDefaultTabBox(Tab tab, TabLayout layout, ResolvedTheme theme, bool isHovered)
        {
            FontProvider.DrawText(TabsHeaderLayoutCalculator.TextSize, isBold: false,
                                  (tab.TabHistory.IsModified ? tab.Name + "*" : tab.Name),
                                  (layout.BoxBounds.Location + layout.TextOffset).ToVector2(),
                                  theme.GetColor(TabsHeaderStyles.TextColor));

            if (isHovered)
            {
                DrawHelper.DrawBackground(layout.BoxBounds, theme.GetColor(TabsHeaderStyles.HoverOverlay));
                DrawHelper.Batch.Draw(theme.GetIcon(TabsHeaderStyles.CloseButton), layout.CloseButtonBounds, Color.White);
            }
        }
    }
}
