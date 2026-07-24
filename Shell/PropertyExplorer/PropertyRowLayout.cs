using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal readonly struct PropertyRowLayout
    {
        public Rectangle LabelBounds { get; init; }
        public Rectangle ValueBounds { get; init; }
        public Rectangle DropdownButtonBounds { get; init; }
    }
}
