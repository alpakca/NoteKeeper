using NoteKeeper.InspectionWindow;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell
{
    internal class TabVisual
    {
        public int Width { get; set; }
        public int XOffset { get; set; }
        public int YOffset { get; set; }

        public int RightPosition => XOffset + Width;
    }
}
