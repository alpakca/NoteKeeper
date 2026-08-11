using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class MenuItemStyleProvider
    {
        private ContextMenuStyle _style;

        public ContextMenuStyle Style => _style;

        public event Action ThemeChanged;

        public void ApplyTheme(ContextMenuStyle style)
        {
            _style = style;

            ThemeChanged?.Invoke();
        }

        public MenuItemStyle GetStyle(IContextMenuEntry entry)
        {
            return _style.ItemStyles[entry.GetType()];
        }
    }
}
