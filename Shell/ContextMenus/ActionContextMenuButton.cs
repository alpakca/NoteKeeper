using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ActionContextMenuButton : IContextMenuItem, IHasRightText
    {
        private Point size;

        private readonly Action _action;

        public Point Location { get; set; }
        public Point Size => size;

        public string? RightText { get; init; }
        public SpriteFont? RightTextFont { get; init; }
        public Color RightTextColor { get; init; }

        public ActionContextMenuButton(Action action)
        {
            _action = action;
        }

        public void Selected()
        {
            _action();
        }

        public void SetActualSize(Point size)
        {
            this.size = size;
        }
    }
}
