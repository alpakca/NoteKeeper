using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using PopUpUI.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#nullable enable

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class TabInputHandler
    {
        private readonly TabViewport viewport;
        private readonly IList<NotePage> notePages;

        public WritingSession WritingSession { get; }
        public TransformSession TransformSession { get; }
        public PageSwapSession PageSwapSession { get; }

        public WindowInputState InputState { get; set; }

        public TabInputHandler(PageActions pageActions, TabViewport viewport, IList<NotePage> notePages)
        {
            this.viewport = viewport;
            this.notePages = notePages;

            WritingSession = new WritingSession(pageActions);
            TransformSession = new TransformSession(pageActions, viewport, notePages);
            PageSwapSession = new PageSwapSession(pageActions, notePages);
        }

        public IPageContent? HitContent(Point mousePosition)
        {
            (int pageIndex, Point pagePosition) = viewport.ToPagePosition(mousePosition);

            if (pageIndex == -1)
            {
                return null;
            }

            foreach (var content in notePages[pageIndex].Contents.HitOrder)
            {
                Point transformedLocation = PageContentUtil.GetTransformedLocation(content.Location, content.Size).TransformedLocation;
                Point absoluteSize = content.Size.Abs();

                if (transformedLocation.X <= pagePosition.X && pagePosition.X < transformedLocation.X + absoluteSize.X &&
                    transformedLocation.Y <= pagePosition.Y && pagePosition.Y < transformedLocation.Y + absoluteSize.Y)
                {
                    return content;
                }
            }

            return null;
        }

        public int PageIndexOf(IPageContent item)
        {
            for (int i = 0; i < notePages.Count; i++)
            {
                if (notePages[i].Contents.Contains(item))
                {
                    return i;
                }
            }

            return -1;
        }

        public void LeftMouseSelection(PopWindowMouseEventArgs e)
        {
            if (InputState == WindowInputState.Writing)
            {
                IKeyTextReceiverContent? hit = HitContent(e.MousePosition) as IKeyTextReceiverContent;

                WritingSession.WritingContent = hit;
                if (hit != null)
                {
                    WritingSession.CaretPosition = hit.GetMouseCaretPosition(viewport.ToPagePosition(e.MousePosition).PagePosition);
                }
            }
            else if (InputState == WindowInputState.Transform)
            {
                HandleTransformer(e);
            }
            else if (InputState == WindowInputState.SwappingPages)
            {
                HandleSwapper(e);
            }
        }

        public void MouseMove(PopWindowMouseEventArgs e)
        {
            NotePagePosition position = viewport.ToPagePosition(e.MousePosition);
            if (position.PageIndex == -1)
            {
                return;
            }

            if (InputState == WindowInputState.Transform)
            {
                TransformSession.HoldUpdate(position);
            }
            else if (InputState == WindowInputState.SwappingPages)
            {
                PageSwapSession.HoldUpdate(position);
            }
        }

        private void HandleTransformer(PopWindowMouseEventArgs e)
        {
            NotePagePosition position = viewport.ToPagePosition(e.MousePosition);

            if (position.PageIndex == -1)
            {
                return;
            }

            if (e.FirstTimeButtons.HasLeft)
            {
                TransformSession.HoldStart(position);
            }
            else if (e.ReleasedButtons.HasLeft)
            {
                TransformSession.HoldEnded(position);
            }
        }

        private void HandleSwapper(PopWindowMouseEventArgs e)
        {
            NotePagePosition position = viewport.ToPagePosition(e.MousePosition);

            if (position.PageIndex == -1)
            {
                return;
            }

            if (e.FirstTimeButtons.HasLeft)
            {
                PageSwapSession.HoldStart(position);
            }
            else if (e.ReleasedButtons.HasLeft)
            {
                PageSwapSession.HoldEnded(position);
            }
        }

        public void TextInput(PopWindowMouseEventArgs e, char ch)
        {
            WritingSession.InsertChar(ch);
        }

        public void KeyInput(PopWindowMouseEventArgs e, Keys key)
        {
            WritingSession.KeyInput(key);
        }
    }
}
