using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class MenuItemStyle
    {
        public Texture2D Background { get; init; }
        public Texture2D? HoverOverlay { get; init; }
        public Texture2D? HeldOverlay { get; init; }

        public string LeftText { get; init; }
        public SpriteFont LeftTextFont { get; init; }
        public Color LeftTextColor { get; init; }
        public Texture2D? LeftIcon { get; init; }
    }

    internal interface IHasRightText
    {
        public string? RightText { get; }
        public SpriteFont? RightTextFont { get; }
        public Color RightTextColor { get; }
    }

    internal interface IHasRightIcon
    {
        public Texture2D? RightIcon { get; }
    }
}
