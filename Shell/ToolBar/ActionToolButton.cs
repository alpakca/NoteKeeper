using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ActionToolButton : IToolButton
    {
        private readonly Action _clickAction;

        public bool Enabled { get; set; } = true;

        public Texture2D DefaultTexture { get; init; }
        public Texture2D UnenabledTexture { get; init; }
        public Texture2D HoveredTexture { get; init; }
        public Texture2D HeldTexture { get; init; }

        public ActionToolButton(Action clickAction)
        {
            _clickAction = clickAction;
        }

        public void Selected()
        {
            _clickAction.Invoke();
        }

        public Texture2D GetTexture(bool isHovered, bool isHeld)
        {
            if (!Enabled)
                return UnenabledTexture;

            return isHeld ? HeldTexture : (isHovered ? HoveredTexture : DefaultTexture);
        }
    }
}
