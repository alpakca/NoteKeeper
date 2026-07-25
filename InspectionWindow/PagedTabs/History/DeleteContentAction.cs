using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.History
{
    internal class DeleteContentAction : IHistoryAction
    {
        private readonly NotePage _notePage;
        private readonly IPageContent _content;
        private readonly int _contentIndex;

        public string UIDescription => throw new NotImplementedException();

        public DeleteContentAction(NotePage notePage, IPageContent content)
        {
            _notePage = notePage;
            _content = content;
            _contentIndex = notePage.Contents.IndexOf(content);
        }

        public void Run()
        {
            _notePage.Contents.RemoveAt(_contentIndex);
        }

        public void Undo()
        {
            _notePage.Contents.Insert(_contentIndex, _content);
        }
    }
}
