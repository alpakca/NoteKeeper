using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.Styling;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell.UndoRedo
{
    internal class HistoryActionsLayoutCalculator
    {
        private const int FrameThickness = 4;

        public const int HorizontalPadding = 16;
        public const int TextSize = 12;
        public const int SelectedActionIndicatorHeight = 20;
        public const int SelectedActionIndicatorWidth = 4;
        public const int LineHeight = 30;

        private int _actionsWidth;

        public Point ScreenLocation { get; set; }

        public void CalculateActionsWidth(IEnumerable<IHistoryAction> actions)
        {
            int maxWidth = 0;

            foreach (var action in actions)
            {
                maxWidth = Math.Max(maxWidth, (int)FontProvider.MeasureWidth(TextSize, isBold: false, action.UIDescription));
            }

            _actionsWidth = maxWidth + HorizontalPadding * 2;
        }

        public Rectangle GetActionLayout(int index)
        {

            return new Rectangle(ScreenLocation.X, ScreenLocation.Y + index * LineHeight, _actionsWidth, LineHeight);
        }

        public Rectangle BackgroundBounds(int itemCount)
        {
            return new Rectangle(ScreenLocation.X - FrameThickness, ScreenLocation.Y - FrameThickness,
                                 _actionsWidth + 2 * FrameThickness, itemCount * LineHeight + 2 * FrameThickness);
        }
    }
}
