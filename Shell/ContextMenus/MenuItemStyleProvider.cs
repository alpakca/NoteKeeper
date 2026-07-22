using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class MenuItemStyleProvider
    {
        private Dictionary<Type, MenuItemStyle> _styles;

        public void ApplyTheme(Dictionary<Type, MenuItemStyle> styles)
        {
            _styles = styles;
        }

        public MenuItemStyle GetStyle(IContextMenuItem item)
        {
            return _styles[item.GetType()];
        }
    }
}
