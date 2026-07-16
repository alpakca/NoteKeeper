using Microsoft.Xna.Framework;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage
{
    internal class TabWindow : PopWindow
    {
        private readonly TabViewport viewport;
        private readonly TabRenderer renderer;

        public TabWindow(Point size, TabViewport viewport, IList<NotePage> notePages) : base(size)
        {
            this.viewport = viewport;
            renderer = new TabRenderer(viewport, notePages);
        }

        protected override void RenderUpdate()
        {
            renderer.RenderPageBackgrounds();

            renderer.RenderWindowTexture(WindowTexture);
        }
    }
}
