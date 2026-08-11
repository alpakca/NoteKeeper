using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow.MultiPage.Components;
using PopUpUI.Input;
using PopUpUI.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#nullable enable

namespace NoteKeeper.InspectionWindow.MultiPage.Input
{
    internal class TabInputHandler
    {
        private readonly TabViewport viewport;
        private readonly ContentQuery contentQuery;

        public WritingSession WritingSession { get; }
        public TransformSession TransformSession { get; }
        public PageSwapSession PageSwapSession { get; }

        public WindowInputState InputState { get; set; }

        public TabInputHandler(PageActions pageActions, TabViewport viewport, ContentQuery contentQuery, IList<NotePage> notePages)
        {
            this.viewport = viewport;
            this.contentQuery = contentQuery;

            WritingSession = new WritingSession(pageActions);
            TransformSession = new TransformSession(pageActions, viewport, contentQuery, notePages);
            PageSwapSession = new PageSwapSession(pageActions, notePages);
        }

        public void LeftMouseSelection(PopWindowMouseEventArgs e)
        {
            if (InputState == WindowInputState.Writing)
            {
                NotePagePosition position = viewport.ToPagePosition(e.MousePosition);
                IKeyTextReceiverContent? hit = contentQuery.HitContent(position) as IKeyTextReceiverContent;

                WritingSession.WritingContent = hit;
                if (hit != null)
                {
                    WritingSession.CaretPosition = hit.GetMouseCaretPosition(position.PagePosition);
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
