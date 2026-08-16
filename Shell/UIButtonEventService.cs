using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.PUIHelpers;
using PopUpUI.Elements;
using PopUpUI.Elements.Visual;
using PopUpUI.PUI;
using System;

namespace NoteKeeper.Shell
{
    internal class UIButtonEventService
    {
        private readonly VisualClickable _undo;
        private readonly VisualClickable _redo;
        private readonly VisualClickable _historyUndo;
        private readonly VisualClickable _historyRedo;
        private readonly VisualClickable _saveCurrentTab;
        private readonly VisualClickable _saveAllTabs;
        private readonly VisualToggleButton _toolMenu;

        public Clickable[] Buttons { get; }

        public event Action UndoRequested;
        public event Action RedoRequested;

        public event Action<Point> HistoryUndoDescriptionRequested;
        public event Action<Point> HistoryRedoDescriptionRequested;

        public event Action SaveDisplayedTabRequested;
        public event Action SaveAllTabsRequested;

        public event Action<bool> ToolMenuVisibilityChangeRequested;

        public UIButtonEventService()
        {
            PUIInterpreter interpreter = PUIHelper.GetInterpreter(PUIHelper.ShellAssetsFolderPath + "Buttons.pui");
            interpreter.ProcessTokens();

            _undo = interpreter.CreateFromProperties(PUIHelper.VisualClickableInstantiator, "Undo");
            _undo.ClickCommand = new ActionCommander<Clickable>(_ => UndoRequested?.Invoke());

            _redo = interpreter.CreateFromProperties(PUIHelper.VisualClickableInstantiator, "Redo");
            _redo.ClickCommand = new ActionCommander<Clickable>(_ => RedoRequested?.Invoke());

            _historyUndo = interpreter.CreateFromProperties(PUIHelper.VisualClickableInstantiator, "Undo_Descriptions");
            _historyUndo.ClickCommand = new ActionCommander<Clickable>(button => HistoryUndoDescriptionRequested?.Invoke(new Point(button.Boundaries.X, button.Boundaries.Bottom)));

            _historyRedo = interpreter.CreateFromProperties(PUIHelper.VisualClickableInstantiator, "Redo_Descriptions");
            _historyRedo.ClickCommand = new ActionCommander<Clickable>(button => HistoryRedoDescriptionRequested?.Invoke(new Point(button.Boundaries.X, button.Boundaries.Bottom)));

            _saveCurrentTab = interpreter.CreateFromProperties(PUIHelper.VisualClickableInstantiator, "Save_Current_Tab");
            _saveCurrentTab.ClickCommand = new ActionCommander<Clickable>(_ => SaveDisplayedTabRequested?.Invoke());

            _saveAllTabs = interpreter.CreateFromProperties(PUIHelper.VisualClickableInstantiator, "Save_All_Tabs");
            _saveAllTabs.ClickCommand = new ActionCommander<Clickable>(_ => SaveAllTabsRequested?.Invoke());

            _toolMenu = interpreter.CreateFromProperties(PUIHelper.VisualToggleButtonInstantiator, "Tool_Menu");
            _toolMenu.ToggleCommand = new ActionCommander<ToggleButton>(button => ToolMenuVisibilityChangeRequested?.Invoke(button.IsChecked));

            Buttons = [_undo, _redo, _historyUndo, _historyRedo, _saveCurrentTab, _saveAllTabs, _toolMenu];
            foreach (var button in Buttons)
            {
                button.Enabled = false;
            }
        }

        public void DisplayedTabChanged(Tab newTab)
        {
            _saveCurrentTab.Enabled = newTab != null;
            _saveAllTabs.Enabled = newTab != null;
            _toolMenu.Enabled = newTab?.ToolMenu != null;
        }

        public void DisplayedTabHistoryUpdated(TabHistory history)
        {
            _undo.Enabled = history.CanUndo;
            _historyUndo.Enabled = history.CanUndo;

            _redo.Enabled = history.CanRedo;
            _historyRedo.Enabled = history.CanRedo;
        }

        public void SetToolMenuCheck(bool isChecked)
        {
            _toolMenu.SetStatusWithoutCommander(isChecked);
        }
    }
}
