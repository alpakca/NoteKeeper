using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.History
{
    internal class AddPageAction : IHistoryAction
    {
        private int _pageIndex;
        private readonly NotePage _page;
        private readonly IList<NotePage> _notePages;

        public string UIDescription => "Add Page";

        public AddPageAction(NotePage page, IList<NotePage> notePages)
        {
            _page = page;
            _notePages = notePages;
        }

        public void Run()
        {
            _pageIndex = _notePages.Count;
            _notePages.Add(_page);
        }

        public void Undo()
        {
            _notePages.RemoveAt(_pageIndex);
        }
    }
}
