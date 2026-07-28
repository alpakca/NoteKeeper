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

        private readonly PageActions pageActions;
        private readonly TabViewport viewport;
        private readonly TabRenderer renderer;
        private readonly TabInputHandler inputHandler;
        private readonly TransformSession transformSession;

        public TabWindow(Point size, PageActions pageActions, TabViewport viewport, IList<NotePage> notePages) : base(size)
        {
            this.pageActions = pageActions;
            this.viewport = viewport;
            this.notePages = notePages;
            this.random = new Random();

            renderer = new TabRenderer(viewport, notePages);
            inputHandler = new TabInputHandler(pageActions, viewport, notePages);
            transformSession = new TransformSession(pageActions, viewport, notePages);
        }

        public void ModeUpdated(ShellModes mode)
        {
            currentMode = mode;
            inputHandler.CurrentMode = mode;
        }

        protected override void RenderUpdate()
        {
            renderer.RenderPageBackgrounds();

            if (currentMode == ShellModes.Transform && transformSession.NotIdle)
            {
                renderer.RenderTransformerNodes(transformSession.ContentsPageIndex, transformSession.NodePagePositions, transformSession.NodeHitRadius);
            }
            else if (currentMode == ShellModes.Writing && inputHandler.WritingSession.WritingContent != null)
            {
                int pageIndex = inputHandler.PageIndexOf(inputHandler.WritingSession.WritingContent);
                renderer.RenderWritingSession(pageIndex, inputHandler.WritingSession);
            }

            renderer.RenderWindowTexture(WindowTexture);
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            if (currentMode == ShellModes.Transform)
            {
                var pos = viewport.ToPagePosition(e.MousePosition);
                transformSession.HoldUpdate(new NotePagePosition(pos.PageIndex, pos.PagePosition));
            }
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (e.MousePosition.X < 0 || e.MousePosition.X >= this.Boundaries.Width ||
                e.MousePosition.Y < 0 || e.MousePosition.Y >= this.Boundaries.Height)
            {
                return;
            }
                
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

            if (e.FirstTimeButtons.HasLeft)
            {
                transformSession.HoldStart(new NotePagePosition(pageIndex, pagePosition));
            }
            else if (e.ReleasedButtons.HasLeft)
            {
                transformSession.HoldEnded(new NotePagePosition(pageIndex, pagePosition));
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
                    content.SetLocation(pagePosition - content.Size / new Point(2));
                    pageActions.AddContent(notePages[pageIndex], content);
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

                    content.SetLocation(pagePosition - content.Size / new Point(2) + new Point(dx, dy));
                    pageActions.AddContent(notePages[pageIndex], content);
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
