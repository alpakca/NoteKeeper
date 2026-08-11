using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class MenuItemStyleProvider
    {
        private Dictionary<Type, MenuItemStyle> _styles;
        private Texture2D _menuBackground;

        public Texture2D MenuBackground => _menuBackground;

        public event Action ThemeChanged;

        public void ApplyTheme(Dictionary<Type, MenuItemStyle> styles, Texture2D menuBackground)
        {
            _styles = styles;
            _menuBackground = menuBackground;

            ThemeChanged?.Invoke();
        }

        public MenuItemStyle GetStyle(IContextMenuEntry entry)
        {
            return _styles[entry.GetType()];
        }
    }
}
