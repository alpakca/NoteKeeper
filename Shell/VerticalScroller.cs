using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Elements.Visual;
using PopUpUI.Surface;
using System.Collections.Generic;

namespace NoteKeeper.Shell
{
    internal class VerticalScroller : VisualVerticalSlider, ITabUpdateReceiver
    {
        private const int Speed = 10;

        private ITabViewport displayedTabViewport;

        public void DisplayedTabChanged(Tab setTab)
        {
            displayedTabViewport = setTab.TabViewport;
        }

        public override void Update()
        {
            AdjustSliderHeight();
            base.Update();
        }

        public override IEnumerable<UIDrawInfo> GetDrawInfos()
        {
            if (displayedTabViewport == null || displayedTabViewport.TotalContentHeight - displayedTabViewport.UIHeight <= 0)
            {
                return [];
            }

            return base.GetDrawInfos();
        }

        protected override void OnSliding()
        {
            displayedTabViewport?.SetVerticalScroll(ScrollOffset * Speed);
        }

        private void AdjustSliderHeight()
        {
            if (displayedTabViewport == null)
            {
                return;
            }

            int remaining = displayedTabViewport.TotalContentHeight - displayedTabViewport.UIHeight;
            if (remaining > 0)
            {
                SlideTo(displayedTabViewport.ScrolledDistance.Y / Speed);
                Boundaries = new Rectangle(Boundaries.X, Boundaries.Y, Boundaries.Width, Range - remaining / Speed);

                base.OnSliding();
            }
        }
    }
}
