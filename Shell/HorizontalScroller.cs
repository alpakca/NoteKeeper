using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Elements.Visual;
using PopUpUI.Surface;
using System.Collections.Generic;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class HorizontalScroller : VisualHorizontalSlider, ITabUpdateReceiver
    {
        private bool slidedWithViewport = false;
        private ITabViewport? displayedTabViewport;

        public void DisplayedTabChanged(Tab? setTab)
        {
            displayedTabViewport = setTab?.TabViewport;
            AdjustSliderHeight();
        }

        public void ZoomChanged()
        {
            AdjustSliderHeight();
        }

        public void ScrollChanged()
        {
            if (displayedTabViewport != null)
            {
                int thumbTravelDistance = this.Range - this.Boundaries.Width;
                float scrolledXRatio = (float)displayedTabViewport.ScrolledDistance.X / (displayedTabViewport.TotalContentWidth - displayedTabViewport.ViewedWidth);
                int thumbXPosition = (int)(thumbTravelDistance * scrolledXRatio);

                slidedWithViewport = true;
                SlideTo(thumbXPosition);
            }
        }

        public override IEnumerable<UIDrawInfo> GetDrawInfos()
        {
            if (displayedTabViewport == null || displayedTabViewport.TotalContentWidth - displayedTabViewport.ViewedWidth <= 0)
            {
                return [];
            }

            return base.GetDrawInfos();
        }

        protected override void OnSliding()
        {
            if (slidedWithViewport)
            {
                slidedWithViewport = false;
                base.OnSliding();

                return;
            }

            if (displayedTabViewport != null)
            {
                float scrolledRatio = (float)ScrollOffset / (this.Range - this.Boundaries.Width);
                int scrollableDistance = displayedTabViewport.TotalContentWidth - displayedTabViewport.ViewedWidth;

                int offset = (int)(scrollableDistance * scrolledRatio);
                displayedTabViewport.SetHorizontalScroll(offset);
            }

            base.OnSliding();
        }

        private void AdjustSliderHeight()
        {
            if (displayedTabViewport == null)
            {
                return;
            }

            if (displayedTabViewport.ViewedWidth > displayedTabViewport.TotalContentWidth)
            {
                return;
            }

            float thumbVisibleRatio = (float)displayedTabViewport.ViewedWidth / displayedTabViewport.TotalContentWidth;
            int thumbWidth = (int)(this.Range * thumbVisibleRatio);

            int thumbTravelDistance = this.Range - thumbWidth;
            float scrolledXRatio = (float)displayedTabViewport.ScrolledDistance.X / (displayedTabViewport.TotalContentWidth - displayedTabViewport.ViewedWidth);
            int thumbXPosition = (int)(thumbTravelDistance * scrolledXRatio);

            Boundaries = new Rectangle(Boundaries.X, Boundaries.Y, thumbWidth, Boundaries.Height);
            SlideTo(thumbXPosition);
        }
    }
}
