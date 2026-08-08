using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.History
{
    internal class PagesSwappedAction : IHistoryAction
    {
        private readonly int _firstPageIndex;
        private readonly int _secondPageIndex;
        private readonly IList<NotePage> _notePages;

        public string UIDescription => throw new NotImplementedException();

        public PagesSwappedAction(int firstPageIndex, int secondPageIndex, IList<NotePage> notePages)
        {
            _firstPageIndex = firstPageIndex;
            _secondPageIndex = secondPageIndex;
            _notePages = notePages;
        }

        public void Run()
        {
            (_notePages[_firstPageIndex], _notePages[_secondPageIndex]) = (_notePages[_secondPageIndex], _notePages[_firstPageIndex]);
        }

        public void Undo()
        {
            (_notePages[_firstPageIndex], _notePages[_secondPageIndex]) = (_notePages[_secondPageIndex], _notePages[_firstPageIndex]);
        }
    }
}
