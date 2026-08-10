using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.UnsavedTabsWindow
{
    internal class WindowButton
    {
        public string Text { get; init; }
        public Color BackgroundColor { get; init; }

        public Rectangle Bounds { get; init; }
        public CloseTabsResult Result { get; init; }
    }
}
