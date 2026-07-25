using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow.PagedTabs.MultiPage;
using PopUpUI.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal interface IKeyTextReceiverContent : IPageContent
    {
        public string Text { get; set; }
        public ITextProcesserResult LastResult { get; }

        public void InsertText(string text, WritingSession writingSession);
        public void InsertChar(char ch, WritingSession writingSession);
        public void KeyPressed(Keys key, WritingSession writingSession);

        public TextPosition GetMouseCaretPosition(Point pagePosition);
    }
}
