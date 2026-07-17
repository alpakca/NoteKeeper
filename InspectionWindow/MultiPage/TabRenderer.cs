using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage
{
    internal class TabRenderer
    {
        private readonly TabViewport viewport;
        private readonly IList<NotePage> notePages;
        private readonly RenderTarget2D target;

        public TabRenderer(TabViewport viewport, IList<NotePage> notePages)
        {
            this.viewport = viewport;
            this.notePages = notePages;

            target = DrawHelper.CreateRenderTarget(viewport.ViewedWidth, viewport.ViewedHeight);
        }

        public void RenderPageBackgrounds()
        {
            DrawHelper.SetBeginBatch(target);

            Point pageOffset = viewport.FirstPageStartPosition;

            for (int i = 0; i < notePages.Count; i++)
            {
                if (viewport.PageWithOffsetOverlapsScreen(pageOffset)) 
                {
                    DrawHelper.DrawBackground(new Rectangle(new Point((int)(pageOffset.X * viewport.Zoom),
                                                                      (int)(pageOffset.Y * viewport.Zoom)), 
                                                            new Point((int)(viewport.EachPageWidth * viewport.Zoom),
                                                                      (int)(viewport.EachPageHeight * viewport.Zoom))), 
                                              Color.White);
                }

                pageOffset += viewport.MovingDimensionPageStep;
            }

            DrawHelper.EndBatch();
        }

        public void RenderWindowTexture(RenderTarget2D windowTexture)
        {
            DrawHelper.SetBeginBatch(windowTexture);
            DrawHelper.Device.Clear(Color.Black);

            DrawHelper.Batch.Draw(target, new Vector2(), Color.White);

            DrawHelper.EndBatch();
        }
    }
}
