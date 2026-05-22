using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.SinglePage
{
    internal class TabViewport : ITabViewport
    {
        public int PageDefaultWidth { get; set; } = 1000;
        public int PageDefaultHeight { get; set; } = 1800;
        public int HorizontalMargin { get; set; } = 100;
        public int VerticalMargin { get; set; } = 50;

        public float Zoom { get; set; } = 1f;
        public Point ScrolledDistance
        {
            get;
            set
            {
                field = value;

                if (PageDefaultWidth * Zoom + HorizontalMargin > VisualWidth)
                    field.X = Math.Clamp(field.X, (int)(VisualWidth - PageDefaultWidth * Zoom - HorizontalMargin), HorizontalMargin);

                if (PageDefaultHeight * Zoom + VerticalMargin > VisualHeight)
                    field.Y = Math.Clamp(field.Y, (int)(VisualHeight - PageDefaultHeight * Zoom - VerticalMargin), VerticalMargin);
            }
        }

        public int VisualWidth { get; set; }
        public int VisualHeight { get; set; }

        public Point PageTopLeftPosition
        {
            get
            {
                Point position = ScrolledDistance;
                if (PageDefaultWidth * Zoom + HorizontalMargin <= VisualWidth)
                {
                    position.X = (int)((VisualWidth - PageDefaultWidth * Zoom) / 2);
                }

                if (PageDefaultHeight * Zoom + VerticalMargin <= VisualHeight)
                {
                    position.Y = (int)((VisualHeight - PageDefaultHeight * Zoom) / 2);
                }

                return position;
            }
        }

        public TabViewport()
        {
            ScrolledDistance = new Point(HorizontalMargin, VerticalMargin);
        }

        public Point ZoomedPosition(Point position)
        {
            return new Point((int)(position.X * Zoom), (int)(position.Y * Zoom));
        }

        public Point RelativeToPage(Point mousePosition)
        {
            mousePosition -= PageTopLeftPosition;
            return new Point((int)(mousePosition.X * Zoom), (int)(mousePosition.Y * Zoom));
        }
    }
}
