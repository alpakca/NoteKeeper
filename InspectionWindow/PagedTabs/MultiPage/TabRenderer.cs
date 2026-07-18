using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
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

            target = DrawHelper.CreateRenderTarget(viewport.UISize.X, viewport.UISize.Y);
        }

        public void RenderPageBackgrounds()
        {
            foreach (var page in notePages)
            {
                foreach (var content in page.Contents)
                {
                    content.RenderUpdate();
                }
            }

            DrawHelper.SetBeginBatch(target);

            Point pageOffset = viewport.FirstPageStartPosition;

            foreach (var page in notePages)
            {
                if (viewport.PageWithOffsetOverlapsScreen(pageOffset))
                {
                    DrawHelper.DrawBackground(new Rectangle(pageOffset,
                                                            new Point(viewport.EachPageWidth, viewport.EachPageHeight).Times(viewport.Zoom)),
                                              Color.White);
                }

                foreach (var content in page.Contents)
                {
                    content.RenderToPage(pageOffset, viewport.Zoom);
                }

                pageOffset += viewport.MovingDimensionPageStep.Times(viewport.Zoom);
            }

            DrawHelper.EndBatch();
        }

        public void RenderTransformerNodes(int transformerContentPageIndex, IEnumerable<Point> nodePagePositions)
        {
            // corner position of page that transformed content is in
            Point pageOffset = viewport.FirstPageStartPosition + viewport.MovingDimensionPageStep.Times(viewport.Zoom).Times(transformerContentPageIndex);

            DrawHelper.SetBeginBatch(target);

            foreach (var position in nodePagePositions)
            {
                // replace with actual image
                DrawHelper.DrawBackground(new Rectangle(pageOffset + position.Times(viewport.Zoom),
                                                        new Point(PageContentTransformer.NodeLength).Times(viewport.Zoom)),
                                          Color.Black);
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
