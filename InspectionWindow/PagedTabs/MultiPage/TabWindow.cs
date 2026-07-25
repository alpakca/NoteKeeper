using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#nullable enable

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class TabWindow : PopWindow
    {
        private ShellModes currentMode;
        private readonly IList<NotePage> notePages;
        private readonly Random random;

        private readonly TabViewport viewport;
        private readonly TabRenderer renderer;
        private readonly TabInputHandler inputHandler;
        private readonly ContentTransformer transformer;

        public TabWindow(Point size, TabViewport viewport, IList<NotePage> notePages) : base(size)
        {
            this.viewport = viewport;
            this.notePages = notePages;
            this.random = new Random();

            renderer = new TabRenderer(viewport, notePages);
            inputHandler = new TabInputHandler(viewport, notePages);
            transformer = new ContentTransformer(notePages, viewport);
        }

        public void ModeUpdated(ShellModes mode)
        {
            currentMode = mode;
            inputHandler.CurrentMode = mode;
        }

        protected override void RenderUpdate()
        {
            renderer.RenderPageBackgrounds();

            if (currentMode == ShellModes.Transform && transformer.IsTransforming)
            {
                renderer.RenderTransformerNodes(transformer.TransformedContentPageIndex, transformer.NodePagePositions);
            }
            else if (currentMode == ShellModes.Writing && inputHandler.WritingContent != null)
            {
                int pageIndex = inputHandler.PageIndexOf(inputHandler.WritingContent);
                renderer.RenderWritingContentCaretBoundary(inputHandler.WritingContent, pageIndex, 
                                                           inputHandler.CaretContentPosition!.Value, inputHandler.WritingContentLineSpacing!.Value);
            }

            renderer.RenderWindowTexture(WindowTexture);
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            if (currentMode == ShellModes.Transform && transformer.AnyNodeSelected)
            {
                (int pageIndex, Point pagePosition) = viewport.ToPagePosition(e.MousePosition);

                if (pageIndex != -1)
                {
                    transformer.MouseMove(pageIndex, pagePosition);
                }
            }
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                HandleTransformer(e);
            }
            else if (e.FirstTimeButtons.HasLeft)
            {
                inputHandler.LeftMouseSelection(e.MousePosition);
            }
        }

        private void HandleTransformer(PopWindowMouseEventArgs e)
        {
            (int pageIndex, Point pagePosition) = viewport.ToPagePosition(e.MousePosition);

            if (pageIndex == -1)
            {
                return;
            }

            if (!transformer.HitAnyNode(pageIndex, pagePosition))
            {
                IPageContent? hitContent = inputHandler.HitContent(e.MousePosition);
                if (transformer.TransformedContent != hitContent && e.FirstTimeButtons.HasLeft)
                {
                    transformer.SetTransformedContent(hitContent, pageIndex);
                }
            }

            if (!transformer.AnyNodeSelected && e.FirstTimeButtons.HasLeft)
            {
                transformer.MouseHoldStart(pageIndex, pagePosition);
            }
            else if (transformer.AnyNodeSelected && !e.CurrentButtons.HasLeft)
            {
                transformer.MouseHoldEnd();
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
            (int pageIndex, Point pagePosition) = viewport.ToPagePosition(e.MousePosition);

            if (pageIndex == -1)
            {
                return;
            }

            if (fileArgs.Files.Length == 1)
            {
                PageImageContent? content = LoadContent(fileArgs.Files[0]);
                if (content != null)
                {
                    content.Location = pagePosition - content.Size / new Point(2);
                    notePages[pageIndex].Contents.Add(content);
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
                    notePages[pageIndex].Contents.Add(content);
                }
            }
        }

        private PageImageContent? LoadContent(string filePath)
        {
            try
            {
                return ContentGetter.GetImageContent(filePath);
            } catch (InvalidOperationException e)
            {
                Trace.WriteLine(e);

                return null;
            }
        }
    }
}
