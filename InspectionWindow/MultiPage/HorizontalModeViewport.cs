using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage
{
    internal class HorizontalModeViewport : ITabViewport
    {
        private const float MaximumZoomValue = 4f;
        private const float MinimumZoomValue = 0.5f;

        private Point scrolledDistance;

        public float Zoom
        {
            get;
            set => field = Math.Clamp(value, MinimumZoomValue, MaximumZoomValue);
        } = 1f;
        public Point ScrolledDistance => scrolledDistance;

        public int UIWidth { get; set; }
        public int UIHeight { get; set; }

        public int TotalContentHeight => (int)(PageCount * Zoom + 2 * VerticalMargin);
        public int TotalContentWidth => (int)(EachPageWidth * Zoom * (EachPageWidth + PageSpacing) - PageSpacing + 2 * HorizontalMargin);

        // by pixels
        public int EachPageHeight { get; set; }
        public int EachPageWidth { get; set; }
        public int HorizontalMargin { get; set; }
        public int VerticalMargin { get; set; }
        public int PageSpacing { get; set; }

        public int PageCount { get; set; }

        private bool ExceedsUIHeight => TotalContentHeight > UIHeight;
        private bool ExceedsUIWidth => TotalContentWidth > UIWidth;

        public void ScrollHorizontal(int amount)
        {
            SetHorizontalScroll(scrolledDistance.X + amount);
        }

        public void ScrollVertical(int amount)
        {
            SetVerticalScroll(scrolledDistance.Y + amount);
        }

        public void SetHorizontalScroll(int xOffset)
        {
            if (ExceedsUIWidth)
            {
                scrolledDistance.X = Math.Clamp(xOffset, 0, TotalContentWidth - UIWidth);
            }
        }

        public void SetVerticalScroll(int yOffset)
        {
            if (ExceedsUIHeight)
            {
                scrolledDistance.Y = Math.Clamp(yOffset, 0, TotalContentHeight - UIHeight);
            }
        }
    }
}
