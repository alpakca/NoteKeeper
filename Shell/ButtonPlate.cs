using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.PUIHelpers;
using PopUpUI.Elements;
using PopUpUI.Elements.Visual;
using PopUpUI.PUI;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell
{
    internal class ButtonPlate
    {
        private readonly ControlPanel panel;

        private readonly VisualToggleButton _writingModeToggle;
        private readonly VisualToggleButton _transformModeToggle;

        public List<Clickable> Buttons { get; }
        public int MinimumVisualHeight { get; }

        public VisualClickable UndoButton { get; }
        public VisualClickable RedoButton { get; }

        public VisualClickable SaveCurrentTabButton { get; }
        public VisualClickable SaveAllTabsButton { get; }

        public VisualToggleButton ToolMenuToggle { get; }

        public ButtonPlate(ControlPanel panel)
        {
            this.panel = panel;

            Buttons = new List<Clickable>();

            _writingModeToggle = new VisualToggleButton();
            _transformModeToggle = new VisualToggleButton();

            UndoButton = new VisualClickable();
            RedoButton = new VisualClickable();

            SaveCurrentTabButton = new VisualClickable();
            SaveAllTabsButton = new VisualClickable();

            ToolMenuToggle = new VisualToggleButton();

            InitializeButtons();
        }

        private void InitializeButtons()
        {
            PUIInterpreter interpreter = PUIHelper.GetInterpreter(PUIHelper.ShellAssetsFolderPath + "ButtonPlate.pui");
            interpreter.ProcessTokens();

            interpreter.AssignProperties(PUIHelper.VisualToggleButtonAssigner, "Writing_Mode_Toggle", _writingModeToggle);
            interpreter.AssignProperties(PUIHelper.VisualToggleButtonAssigner, "Transform_Mode_Toggle", _transformModeToggle);
            IEnumerable<VisualToggleButton> oppositeButtons = [_writingModeToggle, _transformModeToggle];

            _writingModeToggle.ToggleCommand = new OppositeButtonCommander(oppositeButtons, () => panel.Mode = ShellModes.Writing);
            _transformModeToggle.ToggleCommand = new OppositeButtonCommander(oppositeButtons, () => panel.Mode = ShellModes.Transform);

            interpreter.AssignProperties(PUIHelper.VisualClickableAssigner, "Undo", UndoButton);
            UndoButton.ClickCommand = new ActionCommander<Clickable>(_ =>
            {
                panel.DisplayedTab?.TabHistory.Undo();
            });
            UndoButton.Enabled = false;

            interpreter.AssignProperties(PUIHelper.VisualClickableAssigner, "Redo", RedoButton);
            RedoButton.ClickCommand = new ActionCommander<Clickable>(_ =>
            {
                panel.DisplayedTab?.TabHistory.Redo();
            });
            RedoButton.Enabled = false;

            interpreter.AssignProperties(PUIHelper.VisualClickableAssigner, "Save_Current_Tab", SaveCurrentTabButton);
            SaveCurrentTabButton.ClickCommand = new ActionCommander<Clickable>(_ =>
            {
                panel.DisplayedTab?.TabSerializer.Save();
            });
            SaveCurrentTabButton.Enabled = false;

            interpreter.AssignProperties(PUIHelper.VisualClickableAssigner, "Save_All_Tabs", SaveAllTabsButton);
            SaveAllTabsButton.ClickCommand = new ActionCommander<Clickable>(_ =>
            {
                foreach (var tab in panel.ActiveTabs)
                {
                    tab.TabSerializer.Save();
                }
            });
            SaveAllTabsButton.Enabled = false;

            interpreter.AssignProperties(PUIHelper.VisualToggleButtonAssigner, "Tool_Menu_Toggle", ToolMenuToggle);
            ToolMenuToggle.ToggleCommand = new ActionCommander<ToggleButton>(button =>
            {
                panel.SetToolMenuVisibility(button.IsChecked);
            });
            ToolMenuToggle.Enabled = false;

            Buttons.AddRange(_writingModeToggle, _transformModeToggle, UndoButton, RedoButton, SaveCurrentTabButton, SaveAllTabsButton,
                             ToolMenuToggle);
        }

        public void ShellModeUpdated()
        {
            if (panel.Mode == ShellModes.Writing && !_writingModeToggle.IsChecked)
            {
                _writingModeToggle.SetStatusWithoutCommander(true);
                _transformModeToggle.SetStatusWithoutCommander(false);
            }

            if (panel.Mode == ShellModes.Transform && !_transformModeToggle.IsChecked)
            {
                _transformModeToggle.SetStatusWithoutCommander(true);
                _writingModeToggle.SetStatusWithoutCommander(false);
            }
        }

        private class OppositeButtonCommander : ICommander<ToggleButton>
        {
            private readonly Action _toggleAction;
            private readonly IEnumerable<ToggleButton> _allButtons;

            public OppositeButtonCommander(IEnumerable<ToggleButton> allButtons, Action toggleAction)
            {
                _toggleAction = toggleAction;
                _allButtons = allButtons;
            }

            public void Execute(ToggleButton element)
            {
                if (element.IsChecked)
                {
                    foreach (var button in _allButtons)
                    {
                        button.SetStatusWithoutCommander(false);
                    }

                    _toggleAction();
                }

                // always set the toggled when clicked on it, no matter it was toggled-untoggled before
                element.SetStatusWithoutCommander(true);
            }
        }
    }
}
