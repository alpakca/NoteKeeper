using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Elements;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Diagnostics;

#nullable enable

namespace NoteKeeper.InspectionWindow.PagedTabs.SinglePage
{
    internal class TabWindow : PopWindow
    {
        private ShellModes currentMode;
        private readonly NotePage page;
        private readonly Random random;

        private readonly TabRenderer renderer;
        private readonly TabViewport viewport;
        private readonly ContentTransformer transformer;
        private readonly TabInputHandler inputHandler;

        public TabWindow(NotePage page, TabViewport viewport) : base(new Point(viewport.ViewedWidth, viewport.ViewedHeight))
        {
            this.viewport = viewport;
            this.page = page;
            this.random = new Random();

            transformer = new ContentTransformer();
            renderer = new TabRenderer(page, viewport);
            inputHandler = new TabInputHandler(page, viewport);

            ModeUpdated(ShellModes.Transform);
        }

        protected override void RenderUpdate()
        {
            renderer.RenderPage();

            if (currentMode == ShellModes.Transform && transformer.IsTransforming)
            {
                renderer.RenderTransformerNodes(transformer.NodePagePositions);
            }

            // render to window texture at last
            renderer.RenderToWindowTexture(WindowTexture);
        }

        public void ModeUpdated(ShellModes mode)
        {
            currentMode = mode;
            inputHandler.CurrentMode = mode;
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                if (transformer.AnyNodeSelected)
                {
                    transformer.MouseMove(viewport.ToPagePosition(e.MousePosition));
                }
            }
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                if (!transformer.HitAnyNode(viewport.ToPagePosition(e.MousePosition)))
                {
                    IPageContent? hitContent = inputHandler.HitContent(e.MousePosition);
                    if (transformer.TransformedContent != hitContent && e.FirstTimeButtons.HasLeft)
                    {
                        transformer.SetTransformedContent(hitContent, 0);
                    }
                }

                if (!transformer.AnyNodeSelected && e.FirstTimeButtons.HasLeft)
                {
                    transformer.MouseHoldStart(viewport.ToPagePosition(e.MousePosition));
                }
                else if (transformer.AnyNodeSelected && !e.CurrentButtons.HasLeft)
                {
                    transformer.MouseHoldEnd();
                }
            }
            else if (e.FirstTimeButtons.HasLeft)
            {
                inputHandler.LeftMouseSelection(e.MousePosition);
            }
        }

        public void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e)
        {
            OnMouseButtonStateChange(e);
        }

        protected override void OnTextInput(PopWindowMouseEventArgs e, char ch)
        {
            inputHandler.TextInput(e, ch);
        }

        protected override void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            inputHandler.KeyInput(e, key);
        }

        protected override void OnMouseVerticalScrollChange(PopWindowMouseEventArgs e, int change)
        {
            viewport.ScrollVertical(-change / 6);
        }

        protected override void OnMouseHorizontalScrollChange(PopWindowMouseEventArgs e, int change)
        {
            viewport.ScrollHorizontal(-change / 6);
        }

        protected override void OnFileDrop(PopWindowMouseEventArgs e, FileDropEventArgs fileArgs)
        {
            Point pagePosition = viewport.ToPagePosition(e.MousePosition);

            if (fileArgs.Files.Length == 1)
            {
                PageImageContent? content = LoadContent(fileArgs.Files[0]);
                if (content != null)
                {
                    content.Location = pagePosition - content.Size / new Point(2);
                    page.Contents.Add(content);
                }
            }
            else
            {
                for (int i = 0; i < fileArgs.Files.Length; i++)
                {
                    PageImageContent? content = LoadContent(fileArgs.Files[i]);
                    if (content == null)
                    {
                        continue;
                    }

                    const int MaxOffset = 25;
                    int dx = random.Next(MaxOffset);
                    int dy = random.Next(MaxOffset);

                    content.Location = pagePosition - content.Size / new Point(2) + new Point(dx, dy);
                    page.Contents.Add(content);
                }
            }
        }

        private PageImageContent? LoadContent(string filePath)
        {
            try
            {
                return ContentGetter.GetImageContent(filePath);
            }
            catch (InvalidOperationException e)
            {
                Trace.WriteLine(e);

                return null;
            }
        }
    }
}
