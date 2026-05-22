using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System.Collections.Generic;

namespace NoteKeeper.InspectionWindow.SinglePage
{
    internal class TabRenderer
    {
        private readonly NotePage page;
        private readonly TabViewport viewport;
        private readonly RenderTarget2D pageTexture;

        public TabRenderer(NotePage page, TabViewport viewport)
        {
            this.page = page;
            this.viewport = viewport;
            pageTexture = DrawHelper.CreateRenderTarget(viewport.PageDefaultWidth, viewport.PageDefaultHeight);
        }

        public void RenderWindowTexture(RenderTarget2D target)
        {
            DrawHelper.SetBeginBatch(target);

            DrawHelper.DrawBackground(new Rectangle(default, new Point(viewport.VisualWidth, viewport.VisualHeight)), Color.Black);
            DrawHelper.Batch.Draw(pageTexture, new Rectangle(viewport.PageTopLeftPosition, 
                                                             new Point((int)(viewport.PageDefaultWidth * viewport.Zoom), (int)(viewport.PageDefaultHeight * viewport.Zoom))),
                                  Color.White);

            DrawHelper.EndBatch();
        }

        public void RenderTransformer(IEnumerable<Point> nodeScreenPositions)
        {
            DrawHelper.SetBeginBatch(pageTexture);

            foreach (Point position in nodeScreenPositions)
            {
                // replace with actual image
                DrawHelper.DrawBackground(new Rectangle(position, new Point(PageContentTransformer.NodeLength)), Color.White);
            }

            DrawHelper.EndBatch();
        }

        public void RenderPage()
        {
            DrawHelper.SetBeginBatch(pageTexture);

            DrawHelper.DrawBackground(new Rectangle(Point.Zero, new Point(viewport.PageDefaultWidth, viewport.PageDefaultHeight)), Color.White);

            foreach (var content in page.Contents)
            {
                content.Render(new Point());
            }

            DrawHelper.EndBatch();
        }
    }
}
