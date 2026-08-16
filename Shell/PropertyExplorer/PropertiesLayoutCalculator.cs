using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class PropertiesLayoutCalculator
    {
        private const int MarginBetweenBoxes = 2;
        private const int DropDownOptionsRightMargin = 20;

        public const int TextSize = 12;

        private readonly Point DropdownBoxBounds = new Point(40, 40);
        private readonly Point BoxBounds = new Point(100, 40);
        private readonly Point BoxesStartPositionOffset = new Point(30, 200);

        public Rectangle ScreenBounds { get; set; }

        public Point GetDropdownOptionsSize(IEnumerable<string> options)
        {
            int maxWidth = 0;
            foreach (var option in options)
            {
                int width = (int)_styleProvider.Style.Font.MeasureString(option).X;
                maxWidth = Math.Max(maxWidth, width);
            }

            return new Point(maxWidth + DropDownOptionsRightMargin, _styleProvider.Style.Font.LineSpacing);
        }

        public PropertyRowLayout GetRowLayout(IPropertyEntry entry, int index)
        {
            if (entry == null)
            {
                return new PropertyRowLayout();
            }

            Point startPosition = ScreenBounds.Location + BoxesStartPositionOffset;

            Rectangle label = new Rectangle(startPosition.X, startPosition.Y + index * (MarginBetweenBoxes + BoxBounds.Y), BoxBounds.X, BoxBounds.Y);
            Rectangle value = new Rectangle(label.Right + MarginBetweenBoxes, label.Y, BoxBounds.X, BoxBounds.Y);

            if (entry.HasDropdownOptions)
            {
                return new PropertyRowLayout
                {
                    LabelBounds = label,
                    ValueBounds = value,
                    DropdownButtonBounds = new Rectangle(value.Right - DropdownBoxBounds.X, value.Y, DropdownBoxBounds.X, DropdownBoxBounds.Y)
                };
            }

            return new PropertyRowLayout
            {
                LabelBounds = label,
                ValueBounds = value
            };
        }
    }
}
