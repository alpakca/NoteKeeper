using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.History
{
    internal class MoveContentsAcrossPagesAction : IHistoryAction
    {
        private readonly NotePage _oldPage;
        private readonly NotePage _newPage;
        private readonly IPageContent[] _contents;
        private readonly SortedList<int, IPageContent> _contentsPageIndeces;

        public string UIDescription => "Content Page Changed";

        public MoveContentsAcrossPagesAction(NotePage oldPage, NotePage newPage, IPageContent[] contents)
        {
            _oldPage = oldPage;
            _newPage = newPage;
            _contents = contents;
            _contentsPageIndeces = new SortedList<int, IPageContent>(_contents.Length);

            foreach (var content in _contents)
            {
                _contentsPageIndeces.Add(_oldPage.Contents.GetContentIndex(content), content);
            }
        }

        public void Run()
        {
            // remove starting from biggest index
            for (int i = _contentsPageIndeces.Count - 1; i >= 0; i--)
            {
                _oldPage.Contents.RemoveContentAt(_contentsPageIndeces.GetKeyAtIndex(i));
                _newPage.Contents.PushFront(_contentsPageIndeces.GetValueAtIndex(i));
            }
        }

        public void Undo()
        {
            _newPage.Contents.RemoveFromFrontRange(_contents.Length);
            for (int i = 0; i < _contentsPageIndeces.Count; i++)
            {
                _oldPage.Contents.InsertContent(_contentsPageIndeces.GetKeyAtIndex(i), _contentsPageIndeces.GetValueAtIndex(i));
            }
        }
    }
}
