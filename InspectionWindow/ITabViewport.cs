using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal interface ITabViewport
    {
        public float Zoom { get; set; }
        public Point ScrolledDistance { get; }

        public int VisualWidth { get; set; }
        public int VisualHeight { get; set; }

        public void ScrollHorizontal(int xOffset);
        public void ScrollVertical(int yOffset);
    }
}
