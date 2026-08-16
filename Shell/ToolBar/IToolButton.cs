using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal interface IToolButton
    {
        public bool Enabled { get; }
        public Texture2D DefaultTexture { get; }
        public Texture2D UnenabledTexture { get; }

        public void Selected();
    }
}
