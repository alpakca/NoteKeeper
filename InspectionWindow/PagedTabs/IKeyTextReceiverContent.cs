using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow.PagedTabs.MultiPage;
using PopUpUI.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal interface IKeyTextReceiverContent : IPageContent
    {
        public ITextProcesserResult LastResult { get; }

        public TextPosition InsertText(string text, TextPosition caretPosition, PageActions pageActions);
        public TextPosition InsertChar(char ch, TextPosition caretPosition, PageActions pageActions);
        public TextPosition KeyPressed(Keys key, TextPosition caretPosition, PageActions pageActions);

        public TextPosition GetMouseCaretPosition(Point pagePosition);
    }
}
