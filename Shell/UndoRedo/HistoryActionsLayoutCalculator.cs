using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.UndoRedo
{
    internal class HistoryActionsLayoutCalculator
    {
        private const int FrameThickness = 4;

        private HistoryActionsStyleProvider _styleProvider;
        private int _actionsWidth;

        public Point ScreenLocation { get; set; }

        public HistoryActionsLayoutCalculator(HistoryActionsStyleProvider styleProvider)
        {
            _styleProvider = styleProvider;
        }

        public void CalculateActionsWidth(IEnumerable<IHistoryAction> actions)
        {
            int maxWidth = 0;
            HistoryActionsStyle style = _styleProvider.Style;
            foreach (var action in actions)
            {
                maxWidth = Math.Max(maxWidth, (int)style.Font.MeasureString(action.UIDescription).X);
            }

            _actionsWidth = maxWidth + style.TextHorizontalPadding * 2;
        }

        public Rectangle GetActionLayout(int index)
        {
            HistoryActionsStyle style = _styleProvider.Style;
            return new Rectangle(ScreenLocation.X, ScreenLocation.Y + index * style.Height, _actionsWidth, style.Height);
        }

        public Rectangle BackgroundBounds(int itemCount)
        {
            return new Rectangle(ScreenLocation.X - FrameThickness, ScreenLocation.Y - FrameThickness,
                                 _actionsWidth + 2 * FrameThickness, itemCount * _styleProvider.Style.Height + 2 * FrameThickness);
        }
    }
}
