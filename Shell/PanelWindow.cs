using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.ContextMenus;
using NoteKeeper.Shell.PropertyExplorer;
using NoteKeeper.Shell.TabOrganizer;
using NoteKeeper.Shell.ToolBar;
using NoteKeeper.Shell.UndoRedo;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class PanelWindow : PopWindow, ITabUpdateReceiver
    {
        private Tab? displayedTab;

        private readonly ControlPanel panel;
        private readonly ContextMenuManager contextMenuManager;
        private readonly ToolMenuManager toolMenuManager;
        private readonly TabOrganizeManager tabOrganizeManager;
        private readonly PropertyExplorerManager propertiesScreen;
        private readonly HistoryActionsManager historyActionsManager;
        private readonly KeyboardShortcutManager shortcutManager;
        private readonly UIButtonEventService buttonService;

        public PanelWindow(ControlPanel panel) : base(new BatchDrawOptions { BlendState = BlendState.AlphaBlend })
        {
            this.panel = panel;

            contextMenuManager = new ContextMenuManager();
            toolMenuManager = new ToolMenuManager() { ScreenLocation = new Point(60, 240) };
            tabOrganizeManager = new TabOrganizeManager(panel.ActiveTabs) { ScreenBounds = new Rectangle(20, 168, 1880, 32) };
            propertiesScreen = new PropertyExplorerManager() { ScreenBounds = new Rectangle(1600, 168, 300, 800) };
            historyActionsManager = new HistoryActionsManager();

            shortcutManager = new KeyboardShortcutManager();
            InitializeShortcuts();

            buttonService = new UIButtonEventService();
            buttonService.UndoRequested += () => displayedTab?.TabHistory.Undo();
            buttonService.RedoRequested += () => displayedTab?.TabHistory.Redo();
            buttonService.HistoryUndoDescriptionRequested += screenLocation =>
            {
                if (displayedTab != null)
                    historyActionsManager.Show(displayedTab.TabHistory.UndoableActions, screenLocation);
            };
            buttonService.HistoryRedoDescriptionRequested += screenLocation =>
            {
                if (displayedTab != null)
                    historyActionsManager.Show(displayedTab.TabHistory.RedoableActions, screenLocation);
            };
            buttonService.SaveDisplayedTabRequested += () => displayedTab?.TabSerializer.Save();
            buttonService.SaveAllTabsRequested += () =>
            {
                foreach (var tab in panel.ActiveTabs)
                    tab.TabSerializer.Save();
            };
            buttonService.ToolMenuVisibilityChangeRequested += SetToolMenuVisibility;

            tabOrganizeManager.DisplayedTabChanged += panel.DisplayTab;
            historyActionsManager.ActionsReversed += HistoryActionsManager_ActionsReversed;
            toolMenuManager.MenuVisibilityChanged += buttonService.SetToolMenuCheck;

            Elements.Add(shortcutManager);
            Elements.AddRange(buttonService.Buttons);
        }

        private void InitializeShortcuts()
        {
            KeyboardCombination zoomIn = KeyboardCombination.GetSingleUse([Keys.LeftControl, Keys.Add]);
            zoomIn.FirstTriggerAction = () => displayedTab?.TabViewport.Zoom += 0.1f;

            KeyboardCombination zoomOut = KeyboardCombination.GetSingleUse([Keys.LeftControl, Keys.Subtract]);
            zoomOut.FirstTriggerAction = () => displayedTab?.TabViewport.Zoom -= 0.1f;

            shortcutManager.AddCombinations(zoomIn, zoomOut);
        }

        public void DisplayedTabChanged(Tab? setTab)
        {
            if (displayedTab != null)
            {
                displayedTab.TabHistory.HistoryUpdated -= buttonService.DisplayedTabHistoryUpdated;
                displayedTab.PropertyScreenOpenRequested -= ShowPropertyExplorer;
            }

            displayedTab = setTab;

            if (setTab != null)
            {
                setTab.TabHistory.HistoryUpdated += buttonService.DisplayedTabHistoryUpdated;
                setTab.PropertyScreenOpenRequested += ShowPropertyExplorer;

                buttonService.DisplayedTabHistoryUpdated(setTab.TabHistory);
            }

            buttonService.DisplayedTabChanged(setTab);
            tabOrganizeManager.DisplayedTabUpdated(setTab);

            // switched the open tool menu with new tab's tool menu
            SetToolMenuVisibility(toolMenuManager.IsShown);
        }

        private void ShowPropertyExplorer(IPropertyCollection collection)
        {
            propertiesScreen.Show(collection);
        }

        private void SetToolMenuVisibility(bool isVisible)
        {
            if (isVisible && displayedTab?.ToolMenu != null)
            {
                toolMenuManager.Show(displayedTab.ToolMenu);
            }
            else
            {
                toolMenuManager.Hide();
            }
        }

        private void HistoryActionsManager_ActionsReversed(IEnumerable<IHistoryAction> actions, int count)
        {
            if (displayedTab == null)
            {
                return;
            }

            if (actions == displayedTab.TabHistory.UndoableActions)
            {
                for (int i = 0; i < count; i++)
                    displayedTab.TabHistory.Undo();
            }
            else if (actions == displayedTab.TabHistory.RedoableActions)
            {
                for (int i = 0; i < count; i++)
                    displayedTab.TabHistory.Redo();
            }
        }

        public void ScrollChanged()
        {
        }

        protected override void Update()
        {
            contextMenuManager.Update();
            base.Update();
        }

        protected override void RenderUpdate()
        {
            base.RenderUpdate();

            DrawHelper.SetBeginBatch(WindowTexture, DrawOptions);

            toolMenuManager.Render();

            if (panel.ActiveTabs.Count != 0)
            {
                tabOrganizeManager.Render();
            }

            propertiesScreen.Render();
            historyActionsManager.Render();

            // render context menu on top of everything
            contextMenuManager.Render();

            DrawHelper.EndBatch();
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (e.ReleasedButtons.HasRight)
            {
                bool mouseInsideTabWindow = displayedTab != null && displayedTab.Window.Boundaries.Contains(e.MousePosition);

                ContextMenu? contextMenu = mouseInsideTabWindow ? displayedTab!.GetContextMenu(e.MousePosition - displayedTab.Window.Boundaries.Location)
                                                                : panel.GetContextMenu(e.MousePosition);
                if (contextMenu != null)
                {
                    contextMenuManager.Show(contextMenu, e.MousePosition);
                }
            }

            toolMenuManager.MouseButtonStateChange(e);
            contextMenuManager.MouseButtonStateUpdate(e);
            tabOrganizeManager.MouseButtonStateChange(e);
            propertiesScreen.MouseButtonStateChange(e);
            historyActionsManager.MouseButtonStateChange(e);
            base.OnMouseButtonStateChange(e);
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            toolMenuManager.MousePositionChangeUpdate(e);
            contextMenuManager.MousePositionChangeUpdate(e);
            tabOrganizeManager.MousePositionChangeUpdate(e);
            historyActionsManager.MousePositionChangeUpdate(e);
            base.OnMousePositionChange(e);
        }

        protected override void OnTextInput(PopWindowMouseEventArgs e, char ch)
        {
            propertiesScreen.OnTextInsert(e, ch);
            base.OnTextInput(e, ch);
        }

        protected override void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            propertiesScreen.OnKeyDown(e, key);
            base.OnKeyDown(e, key);
        }
    }
}
