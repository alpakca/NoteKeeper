using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class OptionDropdown
    {
        private readonly PropertiesLayoutCalculator _layoutCalculator;
        private readonly Dictionary<string, Rectangle> _optionBounds;

        private IEnumerable<string> _cachedOptions;

        public IEnumerable<KeyValuePair<string, Rectangle>> OptionBounds => _optionBounds;
        public Rectangle OptionsMenuBounds { get; private set; }

        public OptionDropdown(PropertiesLayoutCalculator layoutCalculator)
        {
            _layoutCalculator = layoutCalculator;
            _optionBounds = new Dictionary<string, Rectangle>();
        }

        public void Open(IEnumerable<string> options, Rectangle dropdownBoxBounds)
        {
            if (options == _cachedOptions)
            {
                return;
            }

            _optionBounds.Clear();

            Point optionSize = _layoutCalculator.GetDropdownOptionsSize(options);
            int x = dropdownBoxBounds.Right - optionSize.X;
            int y = dropdownBoxBounds.Bottom;

            foreach (var option in options)
            {
                _optionBounds.Add(option, new Rectangle(x, y, optionSize.X, optionSize.Y));

                y += optionSize.Y;
            }

            OptionsMenuBounds = new Rectangle(x, dropdownBoxBounds.Bottom, optionSize.X, y - dropdownBoxBounds.Bottom);
            _cachedOptions = options;
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

            return selected != null;
        }
    }
}
