using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.SinglePage
{
    internal class TabViewport : ITabViewport
    {
        private const float MinimumZoomValue = 0.5f;
        private const float MaximumZoomValue = 4f;

        private Point scrolledDistance;

        public int PageDefaultWidth { get; set; } = 700;
        public int PageDefaultHeight { get; set; } = 1000;
        public int HorizontalMargin { get; set; } = 20;
        public int VerticalMargin { get; set; } = 20;

        public float Zoom
        {
            get;
            set
            {
                field = Math.Clamp(value, MinimumZoomValue, MaximumZoomValue);
                ZoomChanged?.Invoke();
            }
        } = 1f;

        public Point ScrolledDistance => scrolledDistance;
        public Point UISize { get; set; }

        public int ViewedWidth => (int)(UISize.X / Zoom);
        public int ViewedHeight => (int)(UISize.Y / Zoom);

        public int TotalContentHeight => PageDefaultHeight + 2 * VerticalMargin;
        public int TotalContentWidth => PageDefaultWidth + 2 * HorizontalMargin;

        public event Action ZoomChanged;
        public event Action ScrollChanged;

        private bool ExceedsUIHeight => TotalContentHeight > ViewedHeight;
        private bool ExceedsUIWidth => TotalContentWidth > ViewedWidth;

        public Point PageTopLeftPosition
        {
            get
            {
                Point position = new Point(HorizontalMargin, VerticalMargin) - scrolledDistance;
                if (!ExceedsUIWidth)
                {
                    position.X = (ViewedWidth - TotalContentWidth) / 2;
                }

                if (!ExceedsUIHeight)
                {
                    position.Y = (ViewedHeight - TotalContentHeight) / 2;
                }

                return position.Times(Zoom);
            }
        }

        public Point ToPagePosition(Point mousePosition)
        {
            mousePosition -= PageTopLeftPosition;
            return new Point((int)(mousePosition.X / Zoom), (int)(mousePosition.Y / Zoom));
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
                scrolledDistance.X = Math.Clamp(xOffset, 0, TotalContentWidth - ViewedWidth);
            }

            ScrollChanged?.Invoke();
        }

        public void SetVerticalScroll(int yOffset)
        {
            if (ExceedsUIHeight)
            {
                scrolledDistance.Y = Math.Clamp(yOffset, 0, TotalContentHeight - ViewedHeight);
            }

            ScrollChanged?.Invoke();
        }
    }
}
