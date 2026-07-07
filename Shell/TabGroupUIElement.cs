using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Elements;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell
{
    internal class TabGroupUIElement : PopElement, ISelectable, IHoldable
    {
        private bool isHeld;

        private readonly TextureDrawInfo drawInfo;
        private readonly TabGroup group;

        public bool IsHeld => isHeld;
        public SelectablePacket SelectablePacket { get; } = new SelectablePacket();
        public HoldablePacket HoldPacket { get; } = new HoldablePacket();

        public Texture2D Texture
        {
            get => drawInfo.Texture;
            set => drawInfo.Texture = value;
        }

        public override Rectangle Boundaries
        {
            set
            {
                drawInfo.DestinationArea = value;
                base.Boundaries = value;
            }
        }

        public TabGroupUIElement(TabGroup group)
        {
            this.group = group;
            drawInfo = new TextureDrawInfo();
        }

        public override void RenderUpdate()
        {
            group.RenderTabs();
        }

        public override IEnumerable<UIDrawInfo> GetDrawInfos()
        {
            return [drawInfo];
        }

        public void OnHolding(PopWindowMouseEventArgs e)
        {
            group.Holding(e.MousePosition);
        }

        public void OnHoldStateChange(PopWindowMouseEventArgs e, bool startedHolding)
        {
            isHeld = startedHolding;

            if (!startedHolding)
            {
                group.HoldRemoved();
            }
        }

        public void OnSelect(PopWindowMouseEventArgs e)
        {
            group.Selected(e.MousePosition);
        }

        public void OnSelectingOutside(PopWindowMouseEventArgs e)
        {
        }

        public bool CanBeHeld(Point point)
        {
            return base.InsideBoundaries(point);
        }

        public bool CanBeSelected(Point point)
        {
            return base.InsideBoundaries(point);
        }
    }
}
