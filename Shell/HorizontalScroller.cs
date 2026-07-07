using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Elements.Visual;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell
{
    internal class HorizontalScroller : VisualHorizontalSlider, ITabUpdateReceiver
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
            if (displayedTabViewport == null || displayedTabViewport.DocumentWidth - displayedTabViewport.UIWidth <= 0)
            {
                return [];
            }

            return base.GetDrawInfos();
        }

        protected override void OnSliding()
        {
            displayedTabViewport?.SetHorizontalScroll(ScrollOffset * Speed);
        }

        private void AdjustSliderHeight()
        {
            if (displayedTabViewport == null)
            {
                return;
            }

            int remaining = displayedTabViewport.DocumentWidth - displayedTabViewport.UIWidth;
            if (remaining > 0)
            {
                SlideTo(displayedTabViewport.ScrolledDistance.X / Speed);
                Boundaries = new Rectangle(Boundaries.X, Boundaries.Y, Range - remaining / Speed, Boundaries.Height);

                base.OnSliding();
            }
        }
    }
}
