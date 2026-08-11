using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ActionContextMenuButton : IContextMenuItem
    {
        private readonly Action _action;

        public Point Location { get; set; }
        public Point Size { get; set; }

        public Texture2D? IconTexture { get; }
        public string? RightText { get; }
        public string LeftText { get; }

        public ActionContextMenuButton(Action action, Texture2D? iconTexture, string leftText, string? rightText)
        {
            _action = action;

            IconTexture = iconTexture;
            RightText = rightText;
            LeftText = leftText;
        }

        public void Selected()
        {
            _action();
        }
    }
}
