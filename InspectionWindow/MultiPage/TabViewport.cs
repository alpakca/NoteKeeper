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
            set => field = Math.Clamp(value, MinimumZoomValue, MaximumZoomValue);
        } = 1f;
        public bool InVerticalMode => inVerticalMode;
        public Point ScrolledDistance => scrolledDistance;

        public int UIWidth { get; set; }
        public int UIHeight { get; set; }

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

        // TODO: Bunu MultiPageTab.NotePages.Count ile bağla
        public int PageCount { get; set; }
        public int MovingDimensionPageLength => inVerticalMode ? EachPageHeight : EachPageWidth;
        public int MovingDimensionScroll => inVerticalMode ? scrolledDistance.Y : scrolledDistance.X;
        public Point MovingDimensionPageStep => inVerticalMode ? new Point(0, EachPageHeight + PageSpacing) : new Point(EachPageWidth + PageSpacing, 0);

        private bool ExceedsUIHeight => ScrollableHeight > 0;
        private bool ExceedsUIWidth => ScrollableWidth > 0;

        private float ScrollableHeight => TotalContentHeight - UIHeight;
        private float ScrollableWidth => TotalContentWidth - UIWidth;

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
                    position.Y = (int)((UIHeight - TotalContentHeight * Zoom) / 2);
                }

                if (!ExceedsUIWidth)
                {
                    position.X = (int)((UIWidth - TotalContentWidth * Zoom) / 2);
                }

                return position;
            }
        }

        public bool PageWithOffsetOverlapsScreen(Point offset)
        {
            if (inVerticalMode)
            {
                return offset.X + EachPageWidth > 0;
            }
            else
            {
                return offset.Y + EachPageHeight > 0;
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

        private int GetAllPagesContentLength(int mainLength, int margin)
        {
            return (int)(Zoom * (PageCount * (mainLength + PageSpacing) - PageSpacing)) + 2 * margin;
        }

        private int GetSinglePageContentLength(int mainLength, int margin)
        {
            return (int)(Zoom * mainLength) + 2 * margin;
        }
    }
}
