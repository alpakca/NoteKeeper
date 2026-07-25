using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.History
{
    internal class PropertyChangeAction<T> : IHistoryAction
    {
        private readonly Action<T> _setAction;
        private readonly T _oldValue;
        private readonly T _newValue;

        public string UIDescription => throw new NotImplementedException();

        public PropertyChangeAction(T oldValue, T newValue, Action<T> setAction)
        {
            _oldValue = oldValue;
            _newValue = newValue;
            _setAction = setAction;
        }

        public void Run()
        {
            _setAction(_newValue);
        }

        public void Undo()
        {
            _setAction(_oldValue);
        }
    }
}
