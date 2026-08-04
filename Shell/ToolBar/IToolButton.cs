using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal interface IToolButton
    {
        public bool Enabled { get; }

        public void Selected();
        public Texture2D GetTexture(bool isHovered, bool isHeld);
    }
}
