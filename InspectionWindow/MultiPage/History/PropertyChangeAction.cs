using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.History
{
    internal class PropertyChangeAction<U, T> : IHistoryAction
    {
        private readonly Action<U, T> _setAction;
        private readonly U _element;
        private readonly T _oldValue;
        private readonly T _newValue;

        public string UIDescription => throw new NotImplementedException();

        public PropertyChangeAction(T oldValue, T newValue, U element, Action<U, T> setAction)
        {
            _oldValue = oldValue;
            _newValue = newValue;
            _element = element;
            _setAction = setAction;
        }

        public void Run()
        {
            _setAction(_element, _newValue);
        }

        public void Undo()
        {
            _setAction(_element, _oldValue);
        }
    }
}
