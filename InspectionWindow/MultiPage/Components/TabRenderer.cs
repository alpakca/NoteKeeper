using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow.MultiPage.Input;
using PopUpUI.Text;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class TabRenderer
    {
        private const int WritingBoundaryOutlineThickness = 2;
        private const int WritingBoundaryOutlineMargin = 4;

        private readonly TabViewport viewport;
        private readonly IList<NotePage> notePages;

        private RenderTarget2D target;

        public TabRenderer(TabViewport viewport, IList<NotePage> notePages)
        {
            this.viewport = viewport;
            this.notePages = notePages;

            viewport.UISizeChanged += Viewport_UISizeChanged;
        }

        private void Viewport_UISizeChanged()
        {
            target?.Dispose();
            target = DrawHelper.CreateRenderTarget(viewport.UISize.X, viewport.UISize.Y);
        }

        public void RenderPageBackgrounds()
        {
            foreach (var page in notePages)
            {
                foreach (var content in page.Contents.DrawOrder)
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

                foreach (var content in page.Contents.DrawOrder)
                {
                    content.RenderToPage(pageOffset, viewport.Zoom);
                }

                pageOffset += viewport.MovingDimensionPageStep.Times(viewport.Zoom);
            }

            DrawHelper.EndBatch();
        }

        public void RenderTransformerNodes(TransformSession transformerSession)
        {
            // corner position of page that transformed content is in
            Point pageOffset = viewport.FirstPageStartPosition + viewport.MovingDimensionPageStep.Times(viewport.Zoom).Times(transformerSession.ContentsPageIndex);
            Point hitRadiusOffset = new Point(-transformerSession.NodeHitRadius);

            Point nodeZoomedSize = new Point(transformerSession.NodeHitRadius * 2).Times(viewport.Zoom);

            DrawHelper.SetBeginBatch(target);

            foreach (var position in transformerSession.NodePagePositions)
            {
                // replace with actual image
                DrawHelper.DrawBackground(new Rectangle(pageOffset + (position + hitRadiusOffset).Times(viewport.Zoom),
                                                        nodeZoomedSize),
                                          Color.Black);
            }

            DrawHelper.EndBatch();
        }

        public void RenderWritingSession(int writingContentPageIndex, WritingSession writingSession)
        {
            Point pageOffset = viewport.FirstPageStartPosition + viewport.MovingDimensionPageStep.Times(viewport.Zoom).Times(writingContentPageIndex);

            (Point transformedLocation, _) = PageContentUtil.GetTransformedLocation(writingSession.WritingContent.Location, writingSession.WritingContent.Size);
            Point contentLocation = pageOffset + transformedLocation.Times(viewport.Zoom);

            const int CaretWidth = 2;
            Point caretContentPosition = writingSession.CaretContentPosition.Times(viewport.Zoom);
            Rectangle caretBounds = new Rectangle(contentLocation + caretContentPosition, new Point(CaretWidth, writingSession.LineSpacing).Times(viewport.Zoom));

            int zoomedMargin = (int)(WritingBoundaryOutlineMargin * viewport.Zoom);
            int zoomedThinkness = (int)(WritingBoundaryOutlineThickness * viewport.Zoom);
            Point absoluteSize = writingSession.WritingContent.Size;

            DrawHelper.SetBeginBatch(target);

            DrawHelper.DrawBackground(caretBounds, Color.Black);

            // top bounds
            DrawHelper.DrawBackground(new Rectangle(contentLocation.X - zoomedMargin, contentLocation.Y - zoomedMargin, absoluteSize.X + zoomedMargin * 2, zoomedThinkness), Color.Black);

            // bottom bounds
            DrawHelper.DrawBackground(new Rectangle(contentLocation.X - zoomedMargin, contentLocation.Y + absoluteSize.Y, absoluteSize.X + zoomedMargin * 2, zoomedThinkness), Color.Black);

            // left bounds
            DrawHelper.DrawBackground(new Rectangle(contentLocation.X - zoomedMargin, contentLocation.Y - zoomedMargin, zoomedThinkness, absoluteSize.Y + zoomedMargin * 2), Color.Black);

            // right bounds
            DrawHelper.DrawBackground(new Rectangle(contentLocation.X + absoluteSize.X, contentLocation.Y - zoomedMargin, zoomedThinkness, absoluteSize.Y + zoomedMargin * 2), Color.Black);

            DrawHelper.EndBatch();
        }

        public void RenderWindowTexture(RenderTarget2D windowTexture)
        {
            DrawHelper.SetBeginBatch(windowTexture);
            DrawHelper.Device.Clear(Color.Black);

            DrawHelper.Batch.Draw(target, new Vector2(), Color.White);

            DrawHelper.EndBatch();
        }

        public void SavePages(string directoryPath)
        {
            foreach (var page in notePages)
            {
                foreach (var content in page.Contents.DrawOrder)
                {
                    content.RenderUpdate();
                }
            }

            RenderTarget2D pageTarget = DrawHelper.CreateRenderTarget(viewport.EachPageWidth, viewport.EachPageHeight);

            int pageCount = 1;
            foreach (var page in notePages)
            {
                DrawHelper.SetBeginBatch(pageTarget);
                DrawHelper.Device.Clear(Color.White);

                foreach (var content in page.Contents.DrawOrder)
                {
                    content.RenderToPage(new Point(), 1f);
                }

                DrawHelper.EndBatch();

                using FileStream stream = File.Create(Path.Combine(directoryPath, $"Page_{pageCount}.png"));
                pageTarget.SaveAsPng(stream, pageTarget.Width, pageTarget.Height);

                pageCount++;
            }

            pageTarget.Dispose();
        }
    }
}
