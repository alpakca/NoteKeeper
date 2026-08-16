using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.Styling;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;

using LayoutCalculator = NoteKeeper.Shell.UndoRedo.HistoryActionsLayoutCalculator;

namespace NoteKeeper.Shell.UndoRedo
{
    internal class HistoryActionsRenderer
    {
        private readonly LayoutCalculator _layoutCalculator;

        public HistoryActionsRenderer(LayoutCalculator layoutCalculator)
        {
            _layoutCalculator = layoutCalculator;
        }

        public void Render(IEnumerable<IHistoryAction> actions, int hoveredIndex)
        {
            ResolvedTheme theme = ThemeService.CurrentTheme;
            DrawHelper.DrawBorderedBackground(_layoutCalculator.BackgroundBounds(actions.Count()), 
                                              theme.GetColor(HistoryActionsStyles.Border), theme.GetColor(HistoryActionsStyles.Background), 1);

            int i = 0;
            int textPadding = (LayoutCalculator.LineHeight - FontProvider.GetLineSpacing(LayoutCalculator.TextSize, isBold: false)) / 2;
            int indicatorPadding = (LayoutCalculator.LineHeight - LayoutCalculator.SelectedActionIndicatorHeight) / 2;;
            foreach (var action in actions)
            {
                Rectangle bounds = _layoutCalculator.GetActionLayout(i);

                DrawHelper.DrawBackground(new Rectangle(bounds.Location.X, bounds.Location.Y + indicatorPadding,
                                                        LayoutCalculator.SelectedActionIndicatorWidth, LayoutCalculator.SelectedActionIndicatorHeight),
                                          theme.GetColor(HistoryActionsStyles.SelectedActionIndicator));

                FontProvider.DrawText(LayoutCalculator.TextSize, isBold: false, action.UIDescription,
                                      new Vector2(bounds.Location.X + LayoutCalculator.HorizontalPadding, bounds.Location.Y + textPadding), 
                                      theme.GetColor(HistoryActionsStyles.TextColor));

                if (i <= hoveredIndex)
                {
                    DrawHelper.DrawBackground(bounds, theme.GetColor(HistoryActionsStyles.HoverOverlay));
                }

                i++;
            }
        }
    }
}
