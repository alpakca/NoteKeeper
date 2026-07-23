using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal readonly struct MenuItemLayout
    {
        public Rectangle ItemBounds { get; init; }
        public Rectangle LeftIconBounds { get; init; }
        public Rectangle RightIconBounds { get; init; }
        public Vector2 LeftTextPosition { get; init; }
        public Vector2 RightTextPosition { get; init; }
    }
}
