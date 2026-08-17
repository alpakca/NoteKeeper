using NoteKeeper.InspectionWindow;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using NoteKeeper.Shell.ContextMenus;
using NoteKeeper.Shell.ChildWindow;
using System;
using NoteKeeper.Shell.Styling;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class ControlPanel
    {
        private Tab? displayedTab;
        private bool exitAppWasRequested = false;
        private Rectangle clientBounds;

        private readonly PanelWindow panelWindow;
        private readonly CloseTabsWindow closeTabsWindow;

        public TabCollection ActiveTabs { get; }

        public event Action? ExitApp;

        public ControlPanel()
        {
            ActiveTabs = new TabCollection();
            ActiveTabs.TabRemovalRequested += ActiveTabs_TabRemovalRequested;
            ActiveTabs.TabCountChanged += ActiveTabs_TabCountChanged;

            closeTabsWindow = new CloseTabsWindow();
            closeTabsWindow.WindowClosed += CloseTabsWindow_WindowClosed;

            ThemeService.Refresh();
            ThemeService.SetTheme("dark");

            panelWindow = new PanelWindow(this) { Boundaries = new Rectangle(new Point(), WindowOptions.Size) };
            PopWindowManager.ActiveWindows.Add(panelWindow);
        }

        public ContextMenu? GetContextMenu(Point mousePosition)
        {
            return null;
        }

        /// <summary>Rearranges heights of TabGroup, ButtonPlate and DisplayedTab; by chaning DisplayedTab's height and keeping others minimum.</summary>
        public void RearrangeUISizes(Rectangle newBounds)
        {
            panelWindow.ClientBoundsUpdated(newBounds);
            clientBounds = newBounds;
        }

        public void DisplayTab(Tab? tab)
        {
            if (tab == displayedTab)
            {
                return;
            }

            if (displayedTab != null)
            {
                displayedTab.TabViewport.ScrollChanged -= TabViewport_ScrollChanged;

                PopWindowManager.ActiveWindows.Remove(displayedTab.Window);
            }

            if (tab != null)
            {
                tab.TabViewport.ScrollChanged += TabViewport_ScrollChanged;

                PopWindowManager.ActiveWindows.Add(tab.Window);
                PopWindowManager.ActiveWindows.MoveToBottom(tab.Window);
            }

            displayedTab = tab;
            panelWindow.DisplayedTabChanged(tab);
            RearrangeUISizes(clientBounds);
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

        private void TabViewport_ScrollChanged()
        {
            panelWindow.DisplayedTabScrollUpdated();
        }
    }
}
