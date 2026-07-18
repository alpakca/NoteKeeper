using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Elements.Visual;
using PopUpUI.Surface;
using System.Collections.Generic;

namespace NoteKeeper.Shell
{
    internal class VerticalScroller : VisualVerticalSlider, ITabUpdateReceiver
    {
        private bool slidedWithViewport = false;
        private ITabViewport displayedTabViewport;

        public void DisplayedTabChanged(Tab setTab)
        {
            displayedTabViewport = setTab.TabViewport;
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
                int thumbTravelDistance = this.Range - Boundaries.Height;
                float scrolledYRatio = (float)displayedTabViewport.ScrolledDistance.Y / (displayedTabViewport.TotalContentHeight - displayedTabViewport.ViewedHeight);
                int thumbYPosition = (int)(thumbTravelDistance * scrolledYRatio);

                slidedWithViewport = true;
                SlideTo(thumbYPosition);
            }
        }

        public override IEnumerable<UIDrawInfo> GetDrawInfos()
        {
            if (displayedTabViewport == null || displayedTabViewport.TotalContentHeight - displayedTabViewport.ViewedHeight <= 0)
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
                float scrolledRatio = (float)ScrollOffset / (this.Range - this.Boundaries.Height);
                int scrollableDistance = displayedTabViewport.TotalContentHeight - displayedTabViewport.ViewedHeight;

                int offset = (int)(scrollableDistance * scrolledRatio);
                displayedTabViewport.SetVerticalScroll(offset);
            }

            base.OnSliding();
        }

        private void AdjustSliderHeight()
        {
            if (displayedTabViewport == null)
            {
                return;
            }

            if (displayedTabViewport.ViewedHeight > displayedTabViewport.TotalContentHeight)
            {
                return;
            }

            float thumbVisibleRatio = (float)displayedTabViewport.ViewedHeight / displayedTabViewport.TotalContentHeight;
            int thumbHeight = (int)(this.Range * thumbVisibleRatio);

            int thumbTravelDistance = this.Range - thumbHeight;
            float scrolledYRatio = (float)displayedTabViewport.ScrolledDistance.Y / (displayedTabViewport.TotalContentHeight - displayedTabViewport.ViewedHeight);
            int thumbYPosition = (int)(thumbTravelDistance * scrolledYRatio);

            Boundaries = new Rectangle(Boundaries.X, Boundaries.Y, Boundaries.Width, thumbHeight);
            SlideTo(thumbYPosition);
        }
    }
}
