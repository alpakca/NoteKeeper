using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.History
{
    internal class MoveContentsAcrossPagesAction : IHistoryAction
    {
        private readonly NotePage _oldPage;
        private readonly NotePage _newPage;
        private readonly IPageContent[] _contents;
        private readonly SortedList<int, IPageContent> _contentsPageIndeces;

        public string UIDescription => throw new NotImplementedException();

        public MoveContentsAcrossPagesAction(NotePage oldPage, NotePage newPage, IPageContent[] contents)
        {
            _oldPage = oldPage;
            _newPage = newPage;
            _contents = contents;
            _contentsPageIndeces = new SortedList<int, IPageContent>(_contents.Length);

            foreach (var content in _contents)
            {
                _contentsPageIndeces.Add(_oldPage.Contents.IndexOf(content), content);
            }
        }

        public void Run()
        {
            // remove starting from biggest index
            for (int i = _contentsPageIndeces.Count - 1; i >= 0; i--)
            {
                _oldPage.Contents.RemoveAt(_contentsPageIndeces.GetKeyAtIndex(i));
                _newPage.Contents.Add(_contentsPageIndeces.GetValueAtIndex(i));
            }
        }

        public void Undo()
        {
            _newPage.Contents.RemoveRange(_newPage.Contents.Count - 1, _contents.Length);
            for (int i = 0; i < _contentsPageIndeces.Count; i++)
            {
                _oldPage.Contents.Insert(_contentsPageIndeces.GetKeyAtIndex(i), _contentsPageIndeces.GetValueAtIndex(i));
            }
        }
    }
}
