using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal interface ITabViewport
    {
        public float Zoom { get; set; }
        public Point ScrolledDistance { get; }

        public int UIWidth { get; set; }
        public int UIHeight { get; set; }

        public int TotalContentHeight { get; }
        public int TotalContentWidth { get; }

        public void ScrollHorizontal(int amount);
        public void ScrollVertical(int amount);
        public void SetHorizontalScroll(int xOffset);
        public void SetVerticalScroll(int yOffset);
    }
}
