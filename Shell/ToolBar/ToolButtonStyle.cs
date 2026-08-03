using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolButtonStyle
    {
        public Texture2D DefaultTexture { get; init; }
        public Texture2D? UnenabledTexture { get; init; }
    }
}
