using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.PUIHelpers;
using PopUpUI.Elements;
using PopUpUI.Elements.Visual;
using PopUpUI.PUI;
using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NoteKeeper.Shell
{
    internal class ButtonPlate
    {
        private readonly ControlPanel panel;

        public List<Clickable> Buttons { get; }
        public int MinimumVisualHeight { get; }

        public VisualClickable UndoButton { get; }
        public VisualClickable RedoButton { get; }

        public VisualClickable UndoDescriptionsButton { get; }
        public VisualClickable RedoDescriptionsButton { get; }

        public VisualClickable SaveCurrentTabButton { get; }
        public VisualClickable SaveAllTabsButton { get; }

        public VisualToggleButton ToolMenuToggle { get; }

        public ButtonPlate(ControlPanel panel)
        {
            this.panel = panel;

            Buttons = new List<Clickable>();

            UndoButton = new VisualClickable();
            UndoDescriptionsButton = new VisualClickable();

            RedoButton = new VisualClickable();
            RedoDescriptionsButton = new VisualClickable();

            SaveCurrentTabButton = new VisualClickable();
            SaveAllTabsButton = new VisualClickable();

            ToolMenuToggle = new VisualToggleButton();

            InitializeButtons();
        }

        private void InitializeButtons()
        {
            PUIInterpreter interpreter = PUIHelper.GetInterpreter(PUIHelper.ShellAssetsFolderPath + "ButtonPlate.pui");
            interpreter.ProcessTokens();

            interpreter.AssignProperties(PUIHelper.VisualClickableAssigner, "Undo", UndoButton);
            UndoButton.ClickCommand = new ActionCommander<Clickable>(_ =>
            {
                panel.DisplayedTab?.TabHistory.Undo();
            });
            UndoButton.Enabled = false;

            interpreter.AssignProperties(PUIHelper.VisualClickableAssigner, "Undo_Descriptions", UndoDescriptionsButton);
            UndoDescriptionsButton.ClickCommand = new ActionCommander<Clickable>(button =>
            {
                if (panel.DisplayedTab != null)
                {
                    panel.PanelWindow.HistoryActionsManager.Show(panel.DisplayedTab.TabHistory.UndoableActions,
                                                                 new Point(button.Boundaries.X, button.Boundaries.Bottom));
                }
            });
            UndoDescriptionsButton.Enabled = false;

            interpreter.AssignProperties(PUIHelper.VisualClickableAssigner, "Redo", RedoButton);
            RedoButton.ClickCommand = new ActionCommander<Clickable>(_ =>
            {
                panel.DisplayedTab?.TabHistory.Redo();
            });
            RedoButton.Enabled = false;

            interpreter.AssignProperties(PUIHelper.VisualClickableAssigner, "Redo_Descriptions", RedoDescriptionsButton);
            RedoDescriptionsButton.ClickCommand = new ActionCommander<Clickable>(button =>
            {
                if (panel.DisplayedTab != null)
                {
                    panel.PanelWindow.HistoryActionsManager.Show(panel.DisplayedTab.TabHistory.RedoableActions, 
                                                                 new Point(button.Boundaries.X, button.Boundaries.Bottom));
                }
            });
            RedoDescriptionsButton.Enabled = false;

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
            panel.PanelWindow.ToolMenuManager.MenuVisibilityChanged += ToolMenuToggle.SetStatusWithoutCommander;


            Buttons.AddRange(UndoButton, UndoDescriptionsButton, RedoButton, RedoDescriptionsButton, 
                             SaveCurrentTabButton, SaveAllTabsButton, ToolMenuToggle);
        }
    }
}
