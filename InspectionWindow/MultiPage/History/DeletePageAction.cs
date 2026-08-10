using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.History
{
    internal class DeletePageAction : IHistoryAction
    {
        private int _pageIndex;
        private readonly NotePage _page;
        private readonly IList<NotePage> _notePages;

        public string UIDescription => throw new NotImplementedException();

        public DeletePageAction(NotePage page, IList<NotePage> notePages)
        {
            _page = page;
            _notePages = notePages;
        }

        public void Run()
        {
            _pageIndex = _notePages.IndexOf(_page);
            _notePages.RemoveAt(_pageIndex);
        }

        public void Undo()
        {
            _notePages.Insert(_pageIndex, _page);
        }
    }
}
