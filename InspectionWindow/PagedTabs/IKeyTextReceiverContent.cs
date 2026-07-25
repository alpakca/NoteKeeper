using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal interface IKeyTextReceiverContent : IPageContent
    {
        public TextPosition InsertText(string text, TextPosition caretPosition);
        public TextPosition InsertChar(char ch, TextPosition caretPosition);
        public TextPosition KeyPressed(Keys key, TextPosition caretPosition);
        public TextPosition MouseSelected(Point pagePosition);

        public Vector2 GetCaretScreenPosition(TextPosition caretPosition);
    }
}
