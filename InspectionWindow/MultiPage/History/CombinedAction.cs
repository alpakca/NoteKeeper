using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.History
{
    internal class CombinedAction : IHistoryAction
    {
        private readonly IHistoryAction[] _actions;

        public string UIDescription => throw new NotImplementedException();

        public CombinedAction(params IHistoryAction[] actions)
        {
            _actions = actions;
        }

        public void Run()
        {
            for (int i = 0; i < _actions.Length; i++)
                _actions[i].Run();
        }

        public void Undo()
        {
            for (int i = _actions.Length - 1; i >= 0; i--)
                _actions[i].Undo();
        }
    }
}
