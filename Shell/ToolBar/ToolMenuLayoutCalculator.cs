using Microsoft.Xna.Framework;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolMenuLayoutCalculator
    {
        private const int MarginBetweenBoxes = 2;
        private const int MoveBoxHeight = 32;

        public const int ColumnCount = 2;

        private readonly Point BoxSize = new Point(32, 32);

        public Point ScreenLocation { get; set; }

        public Rectangle GetButtonLayout(int row, int column)
        {
            Point location = new Point(ScreenLocation.X + (BoxSize.X + MarginBetweenBoxes) * column + MarginBetweenBoxes,
                                       ScreenLocation.Y + (BoxSize.Y + MarginBetweenBoxes) * row + MarginBetweenBoxes + MoveBoxHeight);

            return new Rectangle(location, BoxSize);
        }

        public Rectangle GetMoveBoxLayout()
        {
            return new Rectangle(ScreenLocation.X, ScreenLocation.Y, GetMenuWidth(ColumnCount), MoveBoxHeight);
        }

        public Rectangle GetCloseButtonLayout()
        {
            int width = GetMenuWidth(ColumnCount);
            Point location = new Point(ScreenLocation.X + width - BoxSize.X, ScreenLocation.Y);

            return new Rectangle(location, BoxSize);
        }

        public Point GetBackgroundSize(int buttonCount)
        {
            int rowCount = buttonCount / ColumnCount + (buttonCount % ColumnCount == 0 ? 0 : 1);

            return new Point(GetMenuWidth(ColumnCount), MoveBoxHeight + rowCount * (BoxSize.Y + MarginBetweenBoxes) + MarginBetweenBoxes);
        }

        private int GetMenuWidth(int columnCount)
        {
            return columnCount * (BoxSize.X + MarginBetweenBoxes) + MarginBetweenBoxes;
        }
    }
}
