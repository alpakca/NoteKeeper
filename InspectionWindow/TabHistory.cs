using NoteKeeper.InspectionWindow.MultiPage.History;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal delegate void TabHistoryCommandApplied(IHistoryAction appliedCommand, bool isUndo);

    internal class TabHistory
    {
        private readonly Stack<IHistoryAction> _undoStack;
        private readonly Stack<IHistoryAction> _redoStack;

        public bool CanUndo => _undoStack.Count > 0;
        public bool CanRedo => _redoStack.Count > 0;

        public IEnumerable<IHistoryAction> UndoableActions => _undoStack;
        public IEnumerable<IHistoryAction> RedoableActions => _redoStack;

        public event Action<TabHistory> HistoryUpdated;
        public event TabHistoryCommandApplied CommandApplied;

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
            CommandApplied?.Invoke(action, isUndo: false); // forward -> redo
        }

        public void Undo()
        {
            if (!CanUndo)
                return;

            IHistoryAction undoAction = _undoStack.Pop();
            undoAction.Undo();

            _redoStack.Push(undoAction);
            HistoryUpdated?.Invoke(this);
            CommandApplied?.Invoke(undoAction, isUndo: true);
        }

        public void Redo()
        {
            if (!CanRedo)
                return;

            IHistoryAction redoAction = _redoStack.Pop();
            redoAction.Run();

            _undoStack.Push(redoAction);
            HistoryUpdated?.Invoke(this);
            CommandApplied?.Invoke(redoAction, isUndo: false);
        }

        public void Clear()
        {
            _undoStack.Clear();
            _redoStack.Clear();

            HistoryUpdated?.Invoke(this);
        }
    }
}
