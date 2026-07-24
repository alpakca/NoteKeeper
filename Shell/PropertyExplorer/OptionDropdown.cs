using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class OptionDropdown
    {
        private bool isActive;

        private readonly PropertiesLayoutCalculator _layoutCalculator;
        private readonly List<(string Option, Rectangle Bounds)> _optionBounds;

        public bool IsActive => isActive;
        public IEnumerable<(string Option, Rectangle Bounds)> OptionBounds => _optionBounds;

        public OptionDropdown(PropertiesLayoutCalculator layoutCalculator)
        {
            _layoutCalculator = layoutCalculator;
            _optionBounds = new List<(string Option, Rectangle Bounds)>();
        }

        public void Open(IEnumerable<string> options, Rectangle dropdownBoxBounds)
        {
            _optionBounds.Clear();

            Point optionSize = _layoutCalculator.GetDropdownOptionsSize(options);

            int x = dropdownBoxBounds.Right - optionSize.X;
            int y = dropdownBoxBounds.Bottom;
            foreach (var option in options)
            {
                _optionBounds.Add((option, new Rectangle(x, y, optionSize.X, optionSize.Y)));

                y += optionSize.Y;
            }

            isActive = true;
        }

        public bool TrySelectOption(Point mousePosition, out string selected)
        {
            selected = null;

            foreach (var (option, bounds) in _optionBounds)
            {
                if (bounds.Contains(mousePosition))
                {
                    selected = option;
                    break;
                }
            }

            isActive = false;
            return selected != null;
        }
    }
}
