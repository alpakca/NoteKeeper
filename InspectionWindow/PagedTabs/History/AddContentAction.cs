using NoteKeeper.InspectionWindow.PagedTabs;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.History
{
    internal class AddContentAction : IHistoryAction
    {
        private readonly NotePage _notePage;
        private readonly IPageContent _content;
        private int _contentIndex;

        public string UIDescription => throw new NotImplementedException();

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
