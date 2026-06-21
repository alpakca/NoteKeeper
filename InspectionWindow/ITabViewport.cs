using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal interface ITabViewport
    {
        public float Zoom { get; set; }
        public Point ScrolledDistance { get; }

        public int VisualWidth { get; set; }
        public int VisualHeight { get; set; }

        public int ActualHeight { get; }
        public int ActualWidth { get; }

        public void ScrollHorizontal(int amount);
        public void ScrollVertical(int amount);
        public void SetHorizontalScroll(int xOffset);
        public void SetVerticalScroll(int yOffset);
    }
}
