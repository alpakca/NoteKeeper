using PopUpUI.Text;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.History
{
    internal class InsertTextAction : IHistoryAction
    {
        private readonly string _oldText;
        private readonly TextPosition _oldCaretPosition;
        private readonly string _newText;
        private readonly TextPosition _newCaretPosition;
        private readonly PageTextContent _content;

        public string UIDescription => throw new NotImplementedException();

        public InsertTextAction(PageTextContent content, string oldText, TextPosition oldCaretPosition, string newText, TextPosition newCaretPosition)
        {
            _content = content;
            _oldText = oldText;
            _oldCaretPosition = oldCaretPosition;
            _newText = newText;
            _newCaretPosition = newCaretPosition;
        }

        public void Run()
        {
            _content.Text = _newText;
            _content.CaretPosition = _newCaretPosition;
        }

        public void Undo()
        {
            _content.Text = _oldText;
            _content.CaretPosition = _oldCaretPosition;
        }
    }
}
