using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Project;
using NoteKeeper.Shell.ChildWindow;
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
using System.IO;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class PanelWindow : PopWindow
    {
        private const int ScrollSpeed = 6;

        private Tab? displayedTab;

        private Rectangle buttonsPanelBounds;
        private Rectangle verticalScrollerBeltBounds;
        private Rectangle horizontalScrollerBeltBounds;
        private const int scrollersBeltWidth = 24;
        private const int scrollerWidth = 10;

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
            uiService.SaveDisplayedTabRequested += () => 
            { 
                if (displayedTab != null)
                    TabService.SaveTabs([displayedTab]); 
            };
            uiService.SaveAllTabsRequested += () => TabService.SaveTabs(panel.ActiveTabs);
            uiService.ToolMenuVisibilityChangeRequested += SetToolMenuVisibility;
            uiService.CreateTabFileWindowRequested += UiService_CreateTabFileWindowRequested;

            tabOrganizeManager.DisplayedTabChanged += panel.DisplayTab;
            historyActionsManager.ActionsReversed += HistoryActionsManager_ActionsReversed;
            toolMenuManager.MenuVisibilityChanged += uiService.SetToolMenuCheck;

            Elements.Add(shortcutManager);
            Elements.AddRange(uiService.Buttons);
            Elements.AddRange([uiService.VerticalScroller, uiService.HorizontalScroller]);
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

        public void DisplayedTabScrollUpdated()
        {
            uiService.DisplayedTabScrollUpdated();
        }

        public void ClientBoundsUpdated(Rectangle newBounds)
        {
            // button positions
            const int UIMargin = 10;
            const int FrameThickness = 4;
            const int BottomMargin = 40;

            const int ButtonHeight = 32;
            int offset = 0;
            foreach (var button in uiService.Buttons)
            {
                button.Boundaries = new Rectangle(UIMargin + offset, UIMargin, button.Boundaries.Width, ButtonHeight);
                offset += button.Boundaries.Width;
            }

            buttonsPanelBounds = new Rectangle(UIMargin - FrameThickness, UIMargin - FrameThickness, 
                                               offset + 2 * FrameThickness, ButtonHeight + 2 * FrameThickness);

            // tabs header position
            Rectangle tabOrganizerBounds = new Rectangle(buttonsPanelBounds.X, buttonsPanelBounds.Bottom + UIMargin,
                                                         newBounds.Width - UIMargin * 2, ButtonHeight);
            tabOrganizeManager.ScreenBounds = tabOrganizerBounds;

            // displayed tab
            Rectangle displayedTabBounds = new Rectangle(buttonsPanelBounds.X, tabOrganizerBounds.Bottom,
                                                         tabOrganizerBounds.Width - scrollersBeltWidth, newBounds.Height - tabOrganizerBounds.Bottom - BottomMargin);
            if (displayedTab != null)
            {
                displayedTab.TabViewport.UISize = displayedTabBounds.Size;
                displayedTab.Window.Boundaries = displayedTabBounds;
            }

            // scrollers
            const int MarginInBelt = (scrollersBeltWidth - scrollerWidth) / 2;

            verticalScrollerBeltBounds = new Rectangle(displayedTabBounds.Right, displayedTabBounds.Y, scrollersBeltWidth, displayedTabBounds.Height);
            uiService.VerticalScroller.Boundaries = new Rectangle(verticalScrollerBeltBounds.X + MarginInBelt, verticalScrollerBeltBounds.Y, scrollerWidth, 1);
            uiService.VerticalScroller.Range = displayedTabBounds.Height;

            horizontalScrollerBeltBounds = new Rectangle(displayedTabBounds.Left, displayedTabBounds.Bottom, displayedTabBounds.Width, scrollersBeltWidth);
            uiService.HorizontalScroller.Boundaries = new Rectangle(horizontalScrollerBeltBounds.X, horizontalScrollerBeltBounds.Y + MarginInBelt, 1, scrollerWidth);
            uiService.HorizontalScroller.Range = displayedTabBounds.Width;
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

        private void UiService_CreateTabFileWindowRequested()
        {
            CreateTabFileWindow.WindowClosed += CreateTabFileWindow_WindowClosed;
            CreateTabFileWindow.Show(@"C:\Users\user\Desktop\");
        }

        private void CreateTabFileWindow_WindowClosed(string? filePath, string tabExtension)
        {
            CreateTabFileWindow.WindowClosed -= CreateTabFileWindow_WindowClosed;

            if (filePath != null)
            {
                string newPath = Path.ChangeExtension(filePath, tabExtension);
                if (File.Exists(newPath))
                {
                    MessageWindow.Show($"Given path already contains a file with path {newPath}. Try again with another path.");
                    return;
                }

                Tab created = TabService.CreateNew(newPath)!;
                var result = created.TabSerializer.Save();

                panel.ActiveTabs.Add(created);
                panel.DisplayTab(created);
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

            if (uiService.VerticalScroller.ShouldDrawn)
                DrawHelper.DrawBackground(new Rectangle(uiService.VerticalScroller.DrawPosition, uiService.VerticalScroller.Boundaries.Size),
                                          theme.GetColor(ColorRegistry.ScrollerColorId));

            if (uiService.HorizontalScroller.ShouldDrawn)
                DrawHelper.DrawBackground(new Rectangle(uiService.HorizontalScroller.DrawPosition, uiService.HorizontalScroller.Boundaries.Size),
                                          theme.GetColor(ColorRegistry.ScrollerColorId));
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

        protected override void OnMouseVerticalScrollChange(PopWindowMouseEventArgs e, int change)
        {
            displayedTab?.TabViewport.ScrollVertical(-change / ScrollSpeed); 
            base.OnMouseVerticalScrollChange(e, change);
        }

        protected override void OnMouseHorizontalScrollChange(PopWindowMouseEventArgs e, int change)
        {
            displayedTab?.TabViewport.ScrollHorizontal(change / ScrollSpeed);
            base.OnMouseHorizontalScrollChange(e, change);
        }
    }
}
