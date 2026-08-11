using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoteKeeper.Shell.UndoRedo
{
    internal class HistoryActionsRenderer
    {
        private readonly HistoryActionsStyleProvider _styleProvider;
        private readonly HistoryActionsLayoutCalculator _layoutCalculator;

        public HistoryActionsRenderer(HistoryActionsStyleProvider styleProvider, HistoryActionsLayoutCalculator layoutCalculator)
        {
            _styleProvider = styleProvider;
            _layoutCalculator = layoutCalculator;
        }

        public void Render(IEnumerable<IHistoryAction> actions, int hoveredIndex)
        {
            HistoryActionsStyle style = _styleProvider.Style;

            DrawHelper.Batch.Draw(style.Background, _layoutCalculator.BackgroundBounds(actions.Count()), Color.White);

            int i = 0;
            int textPadding = (style.Height - style.Font.LineSpacing) / 2;
            int iconPadding = (style.Height - style.SelectedActionsIconSize.Y) / 2;
            foreach (var action in actions)
            {
                Rectangle bounds = _layoutCalculator.GetActionLayout(i);

                DrawHelper.Batch.Draw(style.SelectedActionsIcon, 
                                      new Rectangle(bounds.Location.X, bounds.Location.Y + iconPadding, 
                                                    style.SelectedActionsIconSize.X, style.SelectedActionsIconSize.Y), 
                                      Color.White);

                DrawHelper.Batch.DrawString(style.Font, action.UIDescription, 
                                            new Vector2(bounds.Location.X + style.TextHorizontalPadding, bounds.Location.Y + textPadding), Color.White);

                if (i <= hoveredIndex)
                {
                    DrawHelper.Batch.Draw(style.SelectedActionsHover, bounds, Color.White);
                }

                i++;
            }
        }
    }
}
