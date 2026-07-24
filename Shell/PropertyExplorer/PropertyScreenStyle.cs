using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal class PropertyScreenStyle
    {
        public Texture2D ScreenBackground { get; init; }
        public SpriteFont Font { get; init; }

        public Texture2D BoxBackground { get; init; }
        public Texture2D FocusedBoxBackground { get; init; }

        public Texture2D DropdownButtonTexture { get; init; }
        public Texture2D DropdownOptionBackground { get; init; }
    }
}
