using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class PropertyScreenStyleProvider
    {
        private PropertyScreenStyle _style;

        public PropertyScreenStyle Style => _style;

        public event Action ThemeChanged;

        public void ApplyTheme(PropertyScreenStyle style)
        {
            _style = style;

            ThemeChanged?.Invoke();
        }
    }
}
