using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.History
{
    internal class AddContentAction : IHistoryAction
    {
        private readonly NotePage _notePage;
        private readonly IPageContent _content;
        private int _contentIndex;

        public string UIDescription => "Add Content";

        public AddContentAction(NotePage notePage, IPageContent content)
        {
            _notePage = notePage;
            _content = content;
        }

        public void Run()
        {
            _contentIndex = _notePage.Contents.Count;
            _notePage.Contents.PushFront(_content);
        }

        public void Undo()
        {
            _notePage.Contents.RemoveContentAt(_contentIndex);
        }
    }
}
