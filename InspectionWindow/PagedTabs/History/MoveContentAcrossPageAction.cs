using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.History
{
    internal class MoveContentAcrossPageAction : IHistoryAction
    {
        private readonly NotePage _oldPage;
        private readonly int _oldPageIndex;
        private readonly NotePage _newPage;
        private int _newPageIndex;
        private readonly IPageContent _content;

        public string UIDescription => throw new NotImplementedException();

        public MoveContentAcrossPageAction(NotePage oldPage, NotePage newPage, IPageContent content)
        {
            _oldPage = oldPage;
            _oldPageIndex = oldPage.Contents.IndexOf(content);

            _newPage = newPage;
            _content = content;
        }

        public void Run()
        {
            _oldPage.Contents.RemoveAt(_oldPageIndex);

            _newPageIndex = _newPage.Contents.Count;
            _newPage.Contents.Insert(_newPageIndex, _content);
        }

        public void Undo()
        {
            _newPage.Contents.RemoveAt(_newPageIndex);

            _oldPage.Contents.Insert(_oldPageIndex, _content);
        }
    }
}
