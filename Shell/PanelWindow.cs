using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.ContextMenus;
using NoteKeeper.Shell.PropertyExplorer;
using NoteKeeper.Shell.Styling;
using NoteKeeper.Shell.TabOrganizer;
using NoteKeeper.Shell.ToolBar;
using NoteKeeper.Shell.UndoRedo;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System.Collections.Generic;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class PanelWindow : PopWindow, ITabUpdateReceiver
    {
        private Tab? displayedTab;
        private Rectangle buttonsPanelBounds;

        private readonly ControlPanel panel;
        private readonly ContextMenuManager contextMenuManager;
        private readonly ToolMenuManager toolMenuManager;
        private readonly TabOrganizeManager tabOrganizeManager;
        private readonly PropertyExplorerManager propertiesScreen;
        private readonly HistoryActionsManager historyActionsManager;
        private readonly KeyboardShortcutManager shortcutManager;
        private readonly UIService uiService;

        public PanelWindow(ControlPanel panel) : base(new BatchDrawOptions { BlendState = BlendState.AlphaBlend })
        {
            this.panel = panel;

            contextMenuManager = new ContextMenuManager();
            toolMenuManager = new ToolMenuManager() { ScreenLocation = new Point(60, 240) };
            tabOrganizeManager = new TabOrganizeManager(panel.ActiveTabs);
            propertiesScreen = new PropertyExplorerManager() { ScreenBounds = new Rectangle(1600, 168, 300, 800) };
            historyActionsManager = new HistoryActionsManager();

            shortcutManager = new KeyboardShortcutManager();
            InitializeShortcuts();

            uiService = new UIService();
            uiService.UndoRequested += () => displayedTab?.TabHistory.Undo();
            uiService.RedoRequested += () => displayedTab?.TabHistory.Redo();
            uiService.HistoryUndoDescriptionRequested += screenLocation =>
            {
                if (displayedTab != null)
                    historyActionsManager.Show(displayedTab.TabHistory.UndoableActions, screenLocation);
            };
            uiService.HistoryRedoDescriptionRequested += screenLocation =>
            {
                if (displayedTab != null)
                    historyActionsManager.Show(displayedTab.TabHistory.RedoableActions, screenLocation);
            };
            uiService.SaveDisplayedTabRequested += () => displayedTab?.TabSerializer.Save();
            uiService.SaveAllTabsRequested += () =>
            {
                foreach (var tab in panel.ActiveTabs)
                    tab.TabSerializer.Save();
            };
            uiService.ToolMenuVisibilityChangeRequested += SetToolMenuVisibility;

            tabOrganizeManager.DisplayedTabChanged += panel.DisplayTab;
            historyActionsManager.ActionsReversed += HistoryActionsManager_ActionsReversed;
            toolMenuManager.MenuVisibilityChanged += uiService.SetToolMenuCheck;

            Elements.Add(shortcutManager);
            Elements.AddRange(uiService.Buttons);
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
                displayedTab.TabHistory.HistoryUpdated -= uiService.DisplayedTabHistoryUpdated;
                displayedTab.PropertyScreenOpenRequested -= ShowPropertyExplorer;
            }

            displayedTab = setTab;

            if (setTab != null)
            {
                setTab.TabHistory.HistoryUpdated += uiService.DisplayedTabHistoryUpdated;
                setTab.PropertyScreenOpenRequested += ShowPropertyExplorer;

                uiService.DisplayedTabHistoryUpdated(setTab.TabHistory);
            }

            uiService.DisplayedTabChanged(setTab);
            tabOrganizeManager.DisplayedTabUpdated(setTab);

            // switched the open tool menu with new tab's tool menu
            SetToolMenuVisibility(toolMenuManager.IsShown);
        }

        public void ClientBoundsUpdated(Rectangle newBounds)
        {
            // button positions
            const int UIMargin = 10; // from first button's X: assets/shell/Buttons.pui
            const int FrameThickness = 4;

            const int ButtonLength = 32;
            for (int i = 0; i < uiService.Buttons.Length; i++)
            {
                uiService.Buttons[i].Boundaries = new Rectangle(UIMargin + ButtonLength * i, UIMargin, ButtonLength, ButtonLength);
            }

            buttonsPanelBounds = new Rectangle(UIMargin - FrameThickness, UIMargin - FrameThickness, 
                                               ButtonLength + 2 * FrameThickness, ButtonLength + 2 * FrameThickness);

            // tabs header position
            tabOrganizeManager.ScreenBounds = new Rectangle(buttonsPanelBounds.X, buttonsPanelBounds.Bottom + UIMargin,
                                                            WindowOptions.Width - UIMargin * 2, ButtonLength);
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
            DrawHelper.SetBeginBatch(WindowTexture, DrawOptions);

            RenderUIButtons();
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

        private void RenderUIButtons()
        {
            ResolvedTheme theme = ThemeService.CurrentTheme;
            DrawHelper.DrawBorderedBackground(buttonsPanelBounds, theme.GetColor(ColorRegistry.BorderAccentId), 
                                              theme.GetColor(ColorRegistry.SurfaceId), 1);

            foreach (var button in uiService.Buttons)
            {
                foreach (var drawInfo in button.GetDrawInfos())
                {
                    DrawHelper.Batch.Draw(drawInfo.GetTexture(), drawInfo.DestinationArea, drawInfo.Opacity * Color.White);
                }
            }
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
