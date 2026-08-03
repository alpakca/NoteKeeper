using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolMenuStyle
    {
        public int ColumnCount { get; init; }
        public Texture2D Background { get; init; }
        public Texture2D? MoveBoxTexture { get; init; }
        public Texture2D CloseButtonTexture { get; init; }

        public Dictionary<string, ToolButtonStyle> ButtonStyles { get; init; }
        public Texture2D? ButtonsHoverOverlay { get; init; }
        public Texture2D? ButtonsHeldOverlay { get; init; }
    }
}
