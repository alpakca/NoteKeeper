using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.UndoRedo
{
    internal class HistoryActionsStyle
    {
        public Texture2D Background { get; init; }
        public SpriteFont Font { get; init; }

        public Texture2D SelectedActionsHover { get; init; }
        public Texture2D SelectedActionsIcon { get; init; }
        public Point SelectedActionsIconSize { get; init; }

        public int Height { get; init; }
        public int TextHorizontalPadding { get; init; }
    }
}
