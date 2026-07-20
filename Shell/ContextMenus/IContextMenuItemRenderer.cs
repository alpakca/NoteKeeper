using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal interface IContextMenuItemRenderer
    {
        public int MinimumWidth { get; }
        public int MinimumHeight { get; }

        public void Render(Point totalSize, Point location, MenuItemStatus status);
    }
}
