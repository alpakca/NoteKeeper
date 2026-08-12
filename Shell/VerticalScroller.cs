using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Elements.Visual;
using PopUpUI.Surface;
using System.Collections.Generic;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class VerticalScroller : VisualVerticalSlider, ITabUpdateReceiver
    {
        private bool slidedFromNonUI = false;
        private ITabViewport? displayedTabViewport;

        public void DisplayedTabChanged(Tab? setTab)
        {
            displayedTabViewport = setTab?.TabViewport;
        }

        public void ScrollChanged()
        {
            if (displayedTabViewport != null)
            {
                int thumbTravelDistance = this.Range - Boundaries.Height;
                float scrolledYRatio = (float)displayedTabViewport.ScrolledDistance.Y / (displayedTabViewport.TotalContentHeight - displayedTabViewport.ViewedHeight);
                int thumbYPosition = (int)(thumbTravelDistance * scrolledYRatio);

                slidedFromNonUI = true;
                SlideTo(thumbYPosition);
            }
        }

        public override void Update()
        {
            UpdateSliderBounds();
            base.Update();
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
            // was called to update inherited ScrollOffset
            if (slidedFromNonUI)
            {
                slidedFromNonUI = false;
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

        private void UpdateSliderBounds()
        {
            if (displayedTabViewport == null || displayedTabViewport.ViewedHeight > displayedTabViewport.TotalContentHeight)
            {
                return;
            }

            float thumbVisibleRatio = (float)displayedTabViewport.ViewedHeight / displayedTabViewport.TotalContentHeight;
            int thumbHeight = (int)(this.Range * thumbVisibleRatio);

            int thumbTravelDistance = this.Range - thumbHeight;
            float scrolledYRatio = (float)displayedTabViewport.ScrolledDistance.Y / (displayedTabViewport.TotalContentHeight - displayedTabViewport.ViewedHeight);
            int thumbYPosition = (int)(thumbTravelDistance * scrolledYRatio);

            Boundaries = new Rectangle(Boundaries.X, Boundaries.Y, Boundaries.Width, thumbHeight);

            slidedFromNonUI = true;
            SlideTo(thumbYPosition);
        }
    }
}
