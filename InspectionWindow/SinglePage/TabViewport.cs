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

        public int VisualWidth { get; set; }
        public int VisualHeight { get; set; }

        public int ActualHeight => (int)(PageDefaultHeight * Zoom + 2 * VerticalMargin);
        public int ActualWidth => (int)(PageDefaultWidth * Zoom + 2 * HorizontalMargin);

        private bool ExceedsVisualHeight => ActualHeight > VisualHeight;
        private bool ExceedsVisualWidth => ActualWidth > VisualWidth;

        public Point PageTopLeftPosition
        {
            get
            {
                Point position = new Point(HorizontalMargin, VerticalMargin) - scrolledDistance;
                if (!ExceedsVisualWidth)
                {
                    position.X = (int)((VisualWidth - PageDefaultWidth * Zoom) / 2);
                }

                if (!ExceedsVisualHeight)
                {
                    position.Y = (int)((VisualHeight - PageDefaultHeight * Zoom) / 2);
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
            if (ExceedsVisualWidth)
            {
                scrolledDistance.X = Math.Clamp(scrolledDistance.X + amount, 0, (int)(ActualWidth - VisualWidth));
            }
        }

        public void ScrollVertical(int amount)
        {
            if (ExceedsVisualHeight)
            {
                scrolledDistance.Y = Math.Clamp(scrolledDistance.Y + amount, 0, (int)(ActualHeight - VisualHeight));
            }
        }

        public void SetHorizontalScroll(int xOffset)
        {
            if (ExceedsVisualWidth)
            {
                scrolledDistance.X = Math.Clamp(xOffset, 0, (int)(ActualWidth - VisualWidth));
            }
        }

        public void SetVerticalScroll(int yOffset)
        {
            if (ExceedsVisualHeight)
            {
                scrolledDistance.Y = Math.Clamp(yOffset, 0, (int)(ActualHeight - VisualHeight));
            }
        }
    }
}
