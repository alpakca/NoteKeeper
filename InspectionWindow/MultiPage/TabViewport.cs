using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage
{
    internal class TabViewport : ITabViewport
    {
        private const float MaximumZoomValue = 4f;
        private const float MinimumZoomValue = 0.5f;

        private bool inVerticalMode = true;
        private Point scrolledDistance;

        public float Zoom
        {
            get;
            set
            {
                field = Math.Clamp(value, MinimumZoomValue, MaximumZoomValue);
                ZoomChanged?.Invoke();
            }
        } = 1f;
        public event Action ZoomChanged;

        public bool InVerticalMode => inVerticalMode;
        public Point ScrolledDistance => scrolledDistance;
        public Point UISize { get; set; }

        public int ViewedWidth => (int)(UISize.X / Zoom);
        public int ViewedHeight => (int)(UISize.Y / Zoom);

        public int TotalContentHeight => InVerticalMode ? GetAllPagesContentLength(EachPageHeight, VerticalMargin) 
                                                        : GetSinglePageContentLength(EachPageHeight, VerticalMargin);
        public int TotalContentWidth => InVerticalMode ? GetSinglePageContentLength(EachPageWidth, HorizontalMargin) 
                                                       : GetAllPagesContentLength(EachPageWidth, HorizontalMargin);

        // by pixels
        public int EachPageHeight { get; set; } = 1000;
        public int EachPageWidth { get; set; } = 700;
        public int HorizontalMargin { get; set; } = 20;
        public int VerticalMargin { get; set; } = 20;
        public int PageSpacing { get; set; } = 30;

        public int PageCount { get; set; }
        public int MovingDimensionPageLength => inVerticalMode ? EachPageHeight : EachPageWidth;
        public int MovingDimensionScroll => inVerticalMode ? scrolledDistance.Y : scrolledDistance.X;
        public Point MovingDimensionPageStep => inVerticalMode ? new Point(0, EachPageHeight + PageSpacing) : new Point(EachPageWidth + PageSpacing, 0);

        private bool ExceedsUIHeight => ScrollableHeight > 0;
        private bool ExceedsUIWidth => ScrollableWidth > 0;

        private float ScrollableHeight => TotalContentHeight - ViewedHeight;
        private float ScrollableWidth => TotalContentWidth - ViewedWidth;

        public int FocusedPageIndex
        {
            get
            {
                int offset = 0;
                int movingDimPageLength = MovingDimensionPageLength;
                int end = inVerticalMode ? scrolledDistance.Y : scrolledDistance.X;

                int i = 0;
                while (offset + movingDimPageLength < end)
                {
                    offset += movingDimPageLength + PageSpacing;
                    i++;
                }

                return i;
            }
        }

        public Point FirstPageStartPosition
        {
            get
            {
                Point position = new Point(HorizontalMargin, VerticalMargin) - scrolledDistance;

                if (!ExceedsUIHeight)
                {
                    position.Y = (ViewedHeight - TotalContentHeight) / 2;
                }

                if (!ExceedsUIWidth)
                {
                    position.X = (ViewedWidth - TotalContentWidth) / 2;
                }

                return position.Times(Zoom);
            }
        }

        public bool PageWithOffsetOverlapsScreen(Point offset)
        {
            if (inVerticalMode)
            {
                return offset.Y + EachPageHeight > 0;
            }
            else
            {
                return offset.X + EachPageWidth > 0;
            }
        }

        public Point MoveThroughMovingDimension(Point pos, int amount)
        {
            if (inVerticalMode)
            {
                return new Point(pos.X, pos.Y + amount);
            }
            else
            {
                return new Point(pos.X + amount, pos.Y);
            }
        }

        public void SetOrientation(bool toVerticalMode)
        {
            float verticalScrollPercentage = scrolledDistance.Y / ScrollableHeight;
            float horizontalScrollPercentage = scrolledDistance.X / ScrollableWidth;

            inVerticalMode = toVerticalMode;

            SetVerticalScroll((int)(ScrollableHeight * verticalScrollPercentage));
            SetHorizontalScroll((int)(ScrollableWidth * horizontalScrollPercentage));
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
                scrolledDistance.X = Math.Clamp(xOffset, 0, (int)ScrollableWidth);
            }
        }

        public void SetVerticalScroll(int yOffset)
        {
            if (ExceedsUIHeight)
            {
                scrolledDistance.Y = Math.Clamp(yOffset, 0, (int)ScrollableHeight);
            }
        }

        private int GetAllPagesContentLength(int mainLength, int margin)
        {
            return PageCount * (mainLength + PageSpacing) - PageSpacing + 2 * margin;
        }

        private static int GetSinglePageContentLength(int mainLength, int margin)
        {
            return mainLength + 2 * margin;
        }
    }
}
