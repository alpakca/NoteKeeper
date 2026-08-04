using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.History
{
    internal class DeleteContentAction : IHistoryAction
    {
        private readonly NotePage _notePage;
        private readonly IPageContent _content;
        private int _contentIndex;

        public string UIDescription => throw new NotImplementedException();

        public DeleteContentAction(NotePage notePage, IPageContent content)
        {
            _notePage = notePage;
            _content = content;
        }

        public void Run()
        {
            _contentIndex = _notePage.Contents.GetContentIndex(_content);
            _notePage.Contents.RemoveContentAt(_contentIndex);
        }

        public void Undo()
        {
            _notePage.Contents.InsertContent(_contentIndex, _content);
        }
    }
}
