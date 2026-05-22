using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.SinglePage
{
    internal class TabViewport : ITabViewport
    {
        private Point scrolledDistance;

        public int PageDefaultWidth { get; set; } = 1000;
        public int PageDefaultHeight { get; set; } = 1800;
        public int HorizontalMargin { get; set; } = 100;
        public int VerticalMargin { get; set; } = 50;

        public float Zoom { get; set; } = 1f;
        public Point ScrolledDistance => scrolledDistance;

        public int VisualWidth { get; set; }
        public int VisualHeight { get; set; }

        private bool ExceedsVisualHeight => PageDefaultHeight * Zoom + 2 * VerticalMargin > VisualHeight;
        private bool ExceedsVisualWidth => PageDefaultWidth * Zoom + 2 * HorizontalMargin > VisualWidth;

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

        public void ScrollHorizontal(int xOffset)
        {
            if (ExceedsVisualWidth)
            {
                scrolledDistance.X = Math.Clamp(scrolledDistance.X + xOffset, 0, (int)(PageDefaultWidth * Zoom + 2 * HorizontalMargin - VisualWidth));
            }
        }

        public void ScrollVertical(int yOffset)
        {
            if (ExceedsVisualHeight)
            {
                scrolledDistance.Y = Math.Clamp(scrolledDistance.Y + yOffset, 0, (int)(PageDefaultHeight * Zoom + 2 * VerticalMargin - VisualHeight));
            }
        }
    }
}
