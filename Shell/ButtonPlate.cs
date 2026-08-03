using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using NoteKeeper.InspectionWindow.PagedTabs;
using NoteKeeper.Shell.PUIHelpers;
using PopUpUI.Elements;
using PopUpUI.Elements.Visual;
using PopUpUI.Input;
using PopUpUI.PUI;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace NoteKeeper.Shell
{
    internal class ButtonPlate
    {
        private readonly ControlPanel panel;

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

            VisualToggleButton writingModeToggle = interpreter.CreateFromProperties(PUIHelper.VisualToggleButtonInstantiator, "Writing_Mode_Toggle");
            VisualToggleButton transformModeToggle = interpreter.CreateFromProperties(PUIHelper.VisualToggleButtonInstantiator, "Transform_Mode_Toggle");

            writingModeToggle.ToggleCommand = new OppositeToggleButtonsCommand(transformModeToggle, () => panel.Mode = ShellModes.Writing);
            transformModeToggle.ToggleCommand = new OppositeToggleButtonsCommand(writingModeToggle, () => panel.Mode = ShellModes.Transform);

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

            Buttons.AddRange(writingModeToggle, transformModeToggle, UndoButton, RedoButton, SaveCurrentTabButton, SaveAllTabsButton,
                             ToolMenuToggle);

            ActivateWritingMode();
            transformModeToggle.SetCheckStatus(false); // don't actually need this line; its here in case of broken UI.
        }

        public void ActivateWritingMode()
        {
            (Buttons[0] as ToggleButton).Toggle();
        }

        public void ActivateTransformMode()
        {
            (Buttons[1] as ToggleButton).Toggle();
        }

        private class OppositeToggleButtonsCommand : ICommander<ToggleButton>
        {
            private readonly Action action;
            private readonly ToggleButton oppositeButton;

            public OppositeToggleButtonsCommand(ToggleButton oppositeButton, Action action)
            {
                this.action = action;
                this.oppositeButton = oppositeButton;
            }

            public void Execute(ToggleButton thisButton)
            {
                // This button was toggled even though it was checked
                if (!oppositeButton.IsChecked && !thisButton.IsChecked)
                {
                    thisButton.Toggle(); // toggle back
                    return;
                }

                if (thisButton.IsChecked)
                {
                    action();

                    if (oppositeButton.IsChecked)
                    {
                        oppositeButton.Toggle();
                    }
                }
            }
        }
    }
}
