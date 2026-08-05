using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.TabOrganizer
{
    internal struct TabLayout
    {
        public Point TextOffset { get; init; }
        public Rectangle BoxBounds { get; init; }
        public Rectangle CloseButtonBounds { get; init; }
    }
}
