using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using PopUpUI.Text;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#nullable enable

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class TabInputHandler
    {
        private IKeyTextReceiverContent? currentKeyTextReceiver;
        private TextPosition textReceiverCaretPosition;

        private readonly TabViewport viewport;
        private readonly IList<NotePage> notePages;

        public int? WritingContentLineSpacing => currentKeyTextReceiver?.LastResult.Data.LineSpacing;
        public Point? CaretContentPosition => currentKeyTextReceiver?.LastResult.Caret.ScreenCaretPosition(textReceiverCaretPosition, null).ToPoint();
        public IKeyTextReceiverContent? WritingContent => currentKeyTextReceiver;

        public ShellModes CurrentMode { get; set; }

        public TabInputHandler(TabViewport viewport, IList<NotePage> notePages)
        {
            this.viewport = viewport;
            this.notePages = notePages;
        }

        public IPageContent? HitContent(Point mousePosition)
        {
            (int pageIndex, Point pagePosition) = viewport.ToPagePosition(mousePosition);

            if (pageIndex == -1)
            {
                return null;
            }

            for (int i = notePages[pageIndex].Contents.Count - 1; i >= 0; i--)
            {
                IPageContent content = notePages[pageIndex].Contents[i];
                if (content.Location.X <= pagePosition.X && pagePosition.X < content.Location.X + content.Size.X &&
                    content.Location.Y <= pagePosition.Y && pagePosition.Y < content.Location.Y + content.Size.Y)
                {
                    return content;
                }
            }

            return null;
        }

        public void LeftMouseSelection(Point mousePosition)
        {
            if (CurrentMode == ShellModes.Writing)
            {
                IKeyTextReceiverContent? hit = HitContent(mousePosition) as IKeyTextReceiverContent;

                currentKeyTextReceiver = hit;
                if (hit != null)
                {
                    textReceiverCaretPosition = hit.MouseSelected(viewport.ToPagePosition(mousePosition).PagePosition);
                }
            }
        }

        public void TextInput(PopWindowMouseEventArgs e, char ch)
        {
            if (currentKeyTextReceiver != null)
            {
                textReceiverCaretPosition = currentKeyTextReceiver.InsertChar(ch, textReceiverCaretPosition);
            }
        }

        public void KeyInput(PopWindowMouseEventArgs e, Keys key)
        {
            if (currentKeyTextReceiver != null)
            {
                textReceiverCaretPosition = currentKeyTextReceiver.KeyPressed(key, textReceiverCaretPosition);
            }
        }
    }
}
