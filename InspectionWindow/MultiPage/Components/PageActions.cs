using NoteKeeper.InspectionWindow.MultiPage.History;
using System;

namespace NoteKeeper.InspectionWindow.MultiPage.Components
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

        public void CombinedAction(params IHistoryAction[] actions)
        {
            _history.RunAction(new CombinedAction(actions));
        }

        public void ChangeContentProperty<U, T>(T oldValue, T newValue, U element, Action<U, T> setAction)
        {
            _history.RunAction(new PropertyChangeAction<U, T>(oldValue, newValue, element, setAction));
        }

        public void AddContent(NotePage notePage, IPageContent content)
        {
            _history.RunAction(new AddContentAction(notePage, content));
        }

        public void DeleteContent(NotePage notePage, IPageContent content)
        {
            _history.RunAction(new DeleteContentAction(notePage, content));
        }
    }
}
