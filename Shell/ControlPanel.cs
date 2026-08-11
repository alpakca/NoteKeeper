using NoteKeeper.InspectionWindow;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NoteKeeper.Shell.ContextMenus;
using NoteKeeper.Shell.UnsavedTabsWindow;
using System;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class ControlPanel
    {
        private Tab? displayedTab;
        private bool exitAppWasRequested = false;

        private readonly ButtonPlate buttonPlate;
        private readonly PanelWindow panelWindow;
        private readonly CloseTabsWindow closeTabsWindow;
        private readonly List<ITabUpdateReceiver> tabUpdateReceivers;

        public ShellModes Mode
        {
            get;
            set
            {
                field = value;
                displayedTab?.ShellModeUpdated(value);
                buttonPlate.ShellModeUpdated();
            }
        }
        public Tab? DisplayedTab => displayedTab;
        public TabCollection ActiveTabs { get; }
        public PanelWindow PanelWindow => panelWindow;

        public event Action? ExitApp;

        public ControlPanel()
        {
            tabUpdateReceivers = new List<ITabUpdateReceiver>();

            ActiveTabs = new TabCollection();
            ActiveTabs.TabRemovalRequested += ActiveTabs_TabRemovalRequested;
            ActiveTabs.TabCountChanged += ActiveTabs_TabCountChanged;

            closeTabsWindow = new CloseTabsWindow();
            closeTabsWindow.WindowClosed += CloseTabsWindow_WindowClosed;

            panelWindow = new PanelWindow(this) { Boundaries = new Rectangle(new Point(), WindowOptions.Size) };
            buttonPlate = new ButtonPlate(this);

            InitializeWindow();
            PopWindowManager.ActiveWindows.Add(panelWindow);

            Mode = ShellModes.Transform;
        }

        private void InitializeWindow()
        {
            VerticalScroller verticalScroller = new VerticalScroller()
            {
                Boundaries = new Rectangle(20 + 1880 - 20 - 40, 200, 40, 500),
                Range = 700,
                DefaultAnimation = new Animation2D([Util.GetTexture(1, 1, Color.Azure)]),

                HoldPacket = new PopUpUI.Input.HoldablePacket(),
                SelectablePacket = new PopUpUI.Input.SelectablePacket()
            };

            HorizontalScroller horizontalScroller = new HorizontalScroller()
            {
                Boundaries = new Rectangle(20, 200 + 700, 1880, 40),
                Range = 1880,
                DefaultAnimation = new Animation2D([Util.GetTexture(1, 1, Color.Azure)]),

                HoldPacket = new PopUpUI.Input.HoldablePacket(),
                SelectablePacket = new PopUpUI.Input.SelectablePacket()
            };

            panelWindow.Elements.Add(verticalScroller);
            panelWindow.Elements.Add(horizontalScroller);
            panelWindow.Elements.AddRange(buttonPlate.Buttons);

            tabUpdateReceivers.AddRange(verticalScroller, horizontalScroller, panelWindow);
        }

        public ContextMenu? GetContextMenu(Point mousePosition)
        {
            return null;
        }

        public void SetToolMenuVisibility(bool isVisible)
        {
            panelWindow.SetToolMenuVisibility(isVisible);
        }

        /// <summary>Rearranges heights of TabGroup, ButtonPlate and DisplayedTab; by chaning DisplayedTab's height and keeping others minimum.</summary>
        public void RearrangeUISizes()
        {
            if (displayedTab != null)
            {
                Point uiSize = new Point(WindowOptions.Width - 40, 700);
                displayedTab.TabViewport.UISize = uiSize;
                displayedTab.Window.Boundaries = new Rectangle(20, 200, uiSize.X, uiSize.Y);
            }
        }

        public void DisplayTab(Tab? tab)
        {
            if (tab == displayedTab)
            {
                return;
            }

            if (displayedTab != null)
            {
                displayedTab.ShellModeChangeRequested -= Tab_ShellModeChangeRequested;
                displayedTab.PropertyScreenOpenRequested -= Tab_PropertyScreenOpenRequested;

                displayedTab.TabViewport.ZoomChanged -= TabViewport_ZoomChanged;
                displayedTab.TabViewport.ScrollChanged -= TabViewport_ScrollChanged;

                displayedTab.TabHistory.HistoryUpdated -= TabHistory_HistoryUpdated;

                PopWindowManager.ActiveWindows.Remove(displayedTab.Window);
            }

            if (tab != null)
            {
                tab.ShellModeChangeRequested += Tab_ShellModeChangeRequested;
                tab.PropertyScreenOpenRequested += Tab_PropertyScreenOpenRequested;

                tab.TabViewport.ZoomChanged += TabViewport_ZoomChanged;
                tab.TabViewport.ScrollChanged += TabViewport_ScrollChanged;

                tab.TabHistory.HistoryUpdated += TabHistory_HistoryUpdated;
                tab.TabHistory.Clear();

                PopWindowManager.ActiveWindows.Add(tab.Window);
                PopWindowManager.ActiveWindows.MoveToBottom(tab.Window);
            }

            displayedTab = tab;
            RearrangeUISizes();
            DisplayedTabChanged(tab);
        }

        private void DisplayedTabChanged(Tab? newTab)
        {
            newTab?.ShellModeUpdated(Mode);

            foreach (var receiver in tabUpdateReceivers)
            {
                receiver.DisplayedTabChanged(newTab);
            }

            buttonPlate.SaveCurrentTabButton.Enabled = newTab != null;
            buttonPlate.SaveAllTabsButton.Enabled = newTab != null;
            buttonPlate.ToolMenuToggle.Enabled = newTab?.ToolMenu != null;
        }

        private void Tab_ShellModeChangeRequested(ShellModes mode)
        {
            Mode = mode;
        }

        private void Tab_PropertyScreenOpenRequested(PropertyExplorer.IPropertyCollection collection)
        {
            panelWindow.PropertiesScreen.Show(collection, new Rectangle(1620, 168, 300, 800));
        }

        public bool TryExitApp()
        {
            if (ActiveTabs.HasModifiedTabs)
            {
                exitAppWasRequested = true;
                ActiveTabs.ClearWithAskingSave();

                return false;
            }

            return true;
        }

        private void ActiveTabs_TabRemovalRequested(IEnumerable<Tab> closingTabs)
        {
            panelWindow.Frozen = true;
            displayedTab?.Window.Frozen = true;

            PopWindowManager.ActiveWindows.Add(closeTabsWindow);
            closeTabsWindow.SetNew(closingTabs);
        }

        private void CloseTabsWindow_WindowClosed(CloseTabsResult result)
        {
            panelWindow.Frozen = false;
            displayedTab?.Window.Frozen = false;
            PopWindowManager.ActiveWindows.Remove(closeTabsWindow);

            if (result == CloseTabsResult.Canceled)
            {
                exitAppWasRequested = false;
                return;
            }

            if (result == CloseTabsResult.Save)
            {
                foreach (var tab in closeTabsWindow.ClosingTabs)
                {
                    tab.TabSerializer.Save();
                }
            }

            ActiveTabs.RemoveTabs(closeTabsWindow.ClosingTabs);
            if (exitAppWasRequested)
            {
                ExitApp?.Invoke();
            }
        }

        private void ActiveTabs_TabCountChanged(TabCollection collection)
        {
            if (!collection.Contains(displayedTab!))
            {
                DisplayTab(null);
            }
        }

        private void TabHistory_HistoryUpdated(TabHistory history)
        {
            buttonPlate.UndoButton.Enabled = history.CanUndo;
            buttonPlate.UndoDescriptionsButton.Enabled = history.CanUndo;

            buttonPlate.RedoButton.Enabled = history.CanRedo;
            buttonPlate.RedoDescriptionsButton.Enabled = history.CanRedo;
        }

        private void TabViewport_ScrollChanged()
        {
            foreach (var receiver in tabUpdateReceivers)
            {
                receiver.ScrollChanged();
            }
        }

        private void TabViewport_ZoomChanged()
        {
            foreach (var receiver in tabUpdateReceivers)
            {
                receiver.ZoomChanged();
            }
        }
    }
}
