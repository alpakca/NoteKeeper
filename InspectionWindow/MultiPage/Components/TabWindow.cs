using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow.MultiPage.Input;
using NoteKeeper.Shell;
using NoteKeeper.Shell.ChildWindow;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

#nullable enable

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class TabWindow : PopWindow
    {
        private WindowInputState inputState;

        private readonly IList<NotePage> notePages;
        private readonly Random random;

        private readonly PageActions pageActions;
        private readonly TabViewport viewport;
        private readonly ContentQuery contentQuery;
        private readonly TabRenderer renderer;
        private readonly TabInputHandler inputHandler;

        public TabWindow(PageActions pageActions, TabViewport viewport, ContentQuery contentQuery, TabToolMenu toolMenu, IList<NotePage> notePages)
        {
            this.pageActions = pageActions;
            this.viewport = viewport;
            this.contentQuery = contentQuery;
            this.notePages = notePages;
            this.random = new Random();

            renderer = new TabRenderer(viewport, notePages);
            inputHandler = new TabInputHandler(pageActions, viewport, contentQuery, notePages);

            viewport.UISizeChanged += Viewport_UISizeChanged;

            toolMenu.WindowInputStateChanged += ToolMenu_WindowInputStateChanged;
            toolMenu.TransformerCornerLockStateChanged += isLocked => inputHandler.TransformSession.LockAspectRatioOnCornerDrag = isLocked;
            toolMenu.SavingPagesRequested += Toolmenu_SavingPagesRequested;
        }

        private void Viewport_UISizeChanged()
        {
            this.Boundaries = new Rectangle(this.Boundaries.Size, viewport.UISize);
        }

        private void ToolMenu_WindowInputStateChanged(WindowInputState state)
        {
            inputState = state;
            inputHandler.InputState = state;
        }

        private void Toolmenu_SavingPagesRequested(string directory)
        {
            DrawCallRenderer.Enqueue(() =>
            {
                Directory.CreateDirectory(directory);
                renderer.SavePages(directory);

                MessageWindow.Show($"Pages have been saved to: {directory}");
            });
        }

        protected override void RenderUpdate()
        {
            renderer.RenderPageBackgrounds();

            if (inputState == WindowInputState.Transform && inputHandler.TransformSession.NotIdle)
            {
                renderer.RenderTransformerNodes(inputHandler.TransformSession);
            }
            else if (inputState == WindowInputState.Writing && inputHandler.WritingSession.WritingContent != null)
            {
                int pageIndex = contentQuery.PageIndexOf(inputHandler.WritingSession.WritingContent);
                renderer.RenderWritingSession(pageIndex, inputHandler.WritingSession);
            }

            renderer.RenderWindowTexture(WindowTexture);
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            inputHandler.MouseMove(e);
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (e.MousePosition.X < 0 || e.MousePosition.X >= this.Boundaries.Width ||
                e.MousePosition.Y < 0 || e.MousePosition.Y >= this.Boundaries.Height)
            {
                return;
            }

            inputHandler.LeftMouseSelection(e);
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
                return new PageImageContent(filePath);
            } catch (InvalidOperationException e)
            {
                Trace.WriteLine(e);

                return null;
            }
        }
    }
}
