using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NoteKeeper.InspectionWindow.SinglePage
{
    internal class TabViewport : ITabViewport
    {
        private const float MinimumZoomValue = 0.5f;
        private const float MaximumZoomValue = 4f;

        private Point scrolledDistance;

        public int PageDefaultWidth { get; set; } = 1000;
        public int PageDefaultHeight { get; set; } = 1800;
        public int HorizontalMargin { get; set; } = 100;
        public int VerticalMargin { get; set; } = 50;

        public float Zoom
        {
            get;
            set => field = Math.Clamp(value, MinimumZoomValue, MaximumZoomValue);
        } = 1f;
        public Point ScrolledDistance => scrolledDistance;

        public int UIWidth { get; set; }
        public int UIHeight { get; set; }

        public int TotalContentHeight => (int)(PageDefaultHeight * Zoom + 2 * VerticalMargin);
        public int TotalContentWidth => (int)(PageDefaultWidth * Zoom + 2 * HorizontalMargin);

        private bool ExceedsUIHeight => TotalContentHeight > UIHeight;
        private bool ExceedsUIWidth => TotalContentWidth > UIWidth;

        public Point PageTopLeftPosition
        {
            get
            {
                Point position = new Point(HorizontalMargin, VerticalMargin) - scrolledDistance;
                if (!ExceedsUIWidth)
                {
                    position.X = (int)((UIWidth - PageDefaultWidth * Zoom) / 2);
                }

                if (!ExceedsUIHeight)
                {
                    position.Y = (int)((UIHeight - PageDefaultHeight * Zoom) / 2);
                }

                return position;
            }
        }

        public Point ToPagePosition(Point mousePosition)
        {
            mousePosition -= PageTopLeftPosition;
            return new Point((int)(mousePosition.X * (1 / Zoom)), (int)(mousePosition.Y * (1 / Zoom)));
        }

        public Point ToScreenLocation(Point pagePosition)
        {
            pagePosition = new Point((int)(pagePosition.X * Zoom), (int)(pagePosition.Y * Zoom));
            return pagePosition + PageTopLeftPosition;
        }

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
                scrolledDistance.X = Math.Clamp(xOffset, 0, (int)(TotalContentWidth - UIWidth));
            }
        }

        public void SetVerticalScroll(int yOffset)
        {
            if (ExceedsUIHeight)
            {
                scrolledDistance.Y = Math.Clamp(yOffset, 0, (int)(TotalContentHeight - UIHeight));
            }
        }
    }
}
