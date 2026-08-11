using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenuStyle
    {
        public Texture2D Background { get; init; }
        public Texture2D HoverOverlay { get; init; }
        public Texture2D HeldOverlay { get; init; }

        public SpriteFont Font { get; init; }
        public Dictionary<Type, MenuItemStyle> ItemStyles { get; init; }
    }
}
