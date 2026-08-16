using Microsoft.Xna.Framework;
using NoteKeeper.Shell.Styling;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class PropertyExplorerLayoutCalculator
    {
        private const int FrameThickness = 2;
        private const int MarginBetweenBoxes = 2;
        private const int DropDownOptionsRightMargin = 20;
        private const int BoxHeight = 24;

        public const int TextSize = 12;

        private readonly Point DropdownBoxBounds = new Point(BoxHeight, BoxHeight);
        private readonly Point BoxesStartPositionOffset = new Point(20, 20);

        private Point BoxBounds;

        public Rectangle ScreenBounds
        {
            get;
            set
            {
                field = value;
                BoxBounds = new Point((value.Width - BoxesStartPositionOffset.X * 2 - MarginBetweenBoxes * 2) / 2, BoxHeight);
            }
        }

        public Point GetDropdownOptionsSize(IEnumerable<string> options)
        {
            int maxWidth = 0;
            foreach (var option in options)
            {
                maxWidth = Math.Max(maxWidth, (int)FontProvider.MeasureWidth(TextSize, isBold: false, option));
            }

            return new Point(maxWidth + DropDownOptionsRightMargin, FontProvider.GetLineSpacing(TextSize, isBold: false));
        }

        public Rectangle GetDropdownMenuBounds(OptionDropdown dropdown)
        {
            return new Rectangle(dropdown.OptionsMenuBounds.Location - new Point(FrameThickness),
                                 dropdown.OptionsMenuBounds.Size + new Point(FrameThickness * 2));
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
