using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System.Collections.Generic;

namespace NoteKeeper.InspectionWindow.PagedTabs.SinglePage
{
    internal class TabRenderer
    {
        private readonly NotePage page;
        private readonly TabViewport viewport;
        private readonly RenderTarget2D target;

        public TabRenderer(NotePage page, TabViewport viewport)
        {
            this.page = page;
            this.viewport = viewport;
            target = DrawHelper.CreateRenderTarget(viewport.UISize.X, viewport.UISize.Y);
        }

        public void RenderPage()
        {
            foreach (var content in page.Contents)
            {
                content.RenderUpdate();
            }

            DrawHelper.SetBeginBatch(target);

            Point pageTopLeft = viewport.PageTopLeftPosition;
            DrawHelper.DrawBackground(new Rectangle(pageTopLeft,
                                                    new Point(viewport.PageDefaultWidth, viewport.PageDefaultHeight).Times(viewport.Zoom)),
                                      Color.White);

            foreach (var content in page.Contents)
            {
                content.RenderToPage(pageTopLeft, viewport.Zoom);
            }

            DrawHelper.EndBatch();
        }

        public void RenderTransformerNodes(IEnumerable<Point> nodePagePositions)
        {
            DrawHelper.SetBeginBatch(target);

            Point pageTopLeft = viewport.PageTopLeftPosition;
            foreach (Point position in nodePagePositions)
            {
                // replace with actual image
                DrawHelper.DrawBackground(new Rectangle(pageTopLeft + position.Times(viewport.Zoom), 
                                                        new Point(PageContentTransformer.NodeLength).Times(viewport.Zoom)), 
                                          Color.Black);
            }

            DrawHelper.EndBatch();
        }

        public void RenderToWindowTexture(RenderTarget2D windowTexture)
        {
            DrawHelper.SetBeginBatch(windowTexture);
            DrawHelper.Device.Clear(Color.Black);

            DrawHelper.Batch.Draw(target, new Vector2(), Color.White);

            DrawHelper.EndBatch();
        }
    }
}
