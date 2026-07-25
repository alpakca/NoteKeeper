using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow.PagedTabs.History;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class PageActions
    {
        private readonly TabHistory _history;

        public event TabHistoryCommandApplied CommandApplied;

        public PageActions(TabHistory history)
        {
            _history = history;
            _history.CommandApplied += (command, isUndo) => CommandApplied?.Invoke(command, isUndo);
        }

        public void RunAction(IHistoryAction action)
        {
            _history.RunAction(action);
        }

        public void ChangeContentProperty<T>(T oldValue, T newValue, Action<T> setAction)
        {
            _history.RunAction(new PropertyChangeAction<T>(oldValue, newValue, setAction));
        }

        public void AddContent(NotePage notePage, IPageContent content)
        {
            _history.RunAction(new AddContentAction(notePage, content));
        }

        public void DeleteContent(NotePage notePage, IPageContent content)
        {
            _history.RunAction(new DeleteContentAction(notePage, content));
        }

        public void MoveContentAcrossPages(NotePage oldPage, NotePage newPage, IPageContent content)
        {
            _history.RunAction(new MoveContentAcrossPageAction(oldPage, newPage, content));
        }
    }
}
