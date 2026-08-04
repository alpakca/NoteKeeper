using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolMenuLayoutCalculator
    {
        private const int MarginBetweenBoxes = 2;
        private const int MoveBoxHeight = 32;

        private readonly Point BoxSize = new Point(32, 32);

        private readonly ToolMenuStyleProvider _styleProvider;

        public Point ScreenLocation { get; set; }

        public ToolMenuLayoutCalculator(ToolMenuStyleProvider styleProvider)
        {
            _styleProvider = styleProvider;
        }

        public Rectangle GetButtonLayout(int row, int column)
        {
            Point location = new Point(ScreenLocation.X + (BoxSize.X + MarginBetweenBoxes) * column + MarginBetweenBoxes,
                                       ScreenLocation.Y + (BoxSize.Y + MarginBetweenBoxes) * row + MarginBetweenBoxes + MoveBoxHeight);

            return new Rectangle(location, BoxSize);
        }

        public Rectangle GetMoveBoxLayout()
        {
            return new Rectangle(ScreenLocation.X, ScreenLocation.Y, GetMenuWidth(_styleProvider.Style.ColumnCount), MoveBoxHeight);
        }

        public Rectangle GetCloseButtonLayout()
        {
            int width = GetMenuWidth(_styleProvider.Style.ColumnCount);
            Point location = new Point(ScreenLocation.X + width - BoxSize.X, ScreenLocation.Y);

            return new Rectangle(location, BoxSize);
        }

        public Point GetBackgroundSize(int buttonCount)
        {
            ToolMenuStyle style = _styleProvider.Style;
            int rowCount = buttonCount / style.ColumnCount + (buttonCount % style.ColumnCount == 0 ? 0 : 1);

            return new Point(GetMenuWidth(style.ColumnCount), MoveBoxHeight + rowCount * (BoxSize.Y + MarginBetweenBoxes) + MarginBetweenBoxes);
        }

        private int GetMenuWidth(int columnCount)
        {
            return columnCount * (BoxSize.X + MarginBetweenBoxes) + MarginBetweenBoxes;
        }
    }
}
