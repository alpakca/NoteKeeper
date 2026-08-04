using Microsoft.Xna.Framework;
using System;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal interface ITabViewport
    {
        public float Zoom { get; set; }
        public Point ScrolledDistance { get; }
        public Point UISize { get; set; }

        public int ViewedWidth { get; }
        public int ViewedHeight { get; }

        public int TotalContentHeight { get; }
        public int TotalContentWidth { get; }

        public event Action ZoomChanged;
        public event Action ScrollChanged;
        public event Action UISizeChanged;

        public void ScrollHorizontal(int amount);
        public void ScrollVertical(int amount);
        public void SetHorizontalScroll(int xOffset);
        public void SetVerticalScroll(int yOffset);
    }
}
