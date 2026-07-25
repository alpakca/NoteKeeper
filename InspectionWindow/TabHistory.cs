using NoteKeeper.InspectionWindow.PagedTabs.History;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal class TabHistory
    {
        private readonly Stack<IHistoryAction> _undoStack;
        private readonly Stack<IHistoryAction> _redoStack;

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public IEnumerable<IHistoryAction> UndoableActions => _undoStack;
        public IEnumerable<IHistoryAction> RedoableActions => _redoStack;

        public event Action<TabHistory> HistoryUpdated;

        public TabHistory()
        {
            _undoStack = new Stack<IHistoryAction>();
            _redoStack = new Stack<IHistoryAction>();
        }

        public void RunAction(IHistoryAction action)
        {
            action.Run();

            _undoStack.Push(action);
            _redoStack.Clear();

            HistoryUpdated?.Invoke(this);
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            IHistoryAction undoAction = _undoStack.Pop();
            undoAction.Undo();

            _redoStack.Push(undoAction);
            HistoryUpdated?.Invoke(this);
        }

        public void Redo()
        {
            if (!CanRedo)
                return;

            IHistoryAction redoAction = _redoStack.Pop();
            redoAction.Run();

            _undoStack.Push(redoAction);
            HistoryUpdated?.Invoke(this);
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();

            HistoryUpdated?.Invoke(this);
        }
    }
}
