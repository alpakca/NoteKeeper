using NoteKeeper.InspectionWindow;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;
using NoteKeeper.Shell.ContextMenus;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class ControlPanel
    {
        private Tab? displayedTab;

        private readonly ButtonPlate buttonPlate;
        private readonly TabGroup tabGroup;

        private readonly PanelWindow window;
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
        public ObservableCollection<Tab> ActiveTabs { get; }
        public PanelWindow PanelWindow => window;

        public ControlPanel()
        {
            tabUpdateReceivers = new List<ITabUpdateReceiver>();
            ActiveTabs = new ObservableCollection<Tab>();

            window = new PanelWindow(this) { Boundaries = new Rectangle(new Point(), WindowOptions.Size) };
            buttonPlate = new ButtonPlate(this);
            tabGroup = new TabGroup(this) { Boundaries = new Rectangle(20, 170, 1720, 30) };

            InitializeWindow();
            PopWindowManager.ActiveWindows.Add(window);

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

            window.Elements.Add(verticalScroller);
            window.Elements.Add(horizontalScroller);
            window.Elements.AddRange(buttonPlate.Buttons);
            window.Elements.Add(tabGroup.UIElement);

            tabUpdateReceivers.AddRange(verticalScroller, horizontalScroller, window);
        }

        public ContextMenu? GetContextMenu(Point mousePosition)
        {
            return null;
        }

        public void SetToolMenuVisibility(bool isVisible)
        {
            window.SetToolMenuVisibility(isVisible);
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

                displayedTab.TabViewport.ZoomChanged -= TabViewport_ZoomChanged;
                displayedTab.TabViewport.ScrollChanged -= TabViewport_ScrollChanged;

                displayedTab.TabHistory.HistoryUpdated -= TabHistory_HistoryUpdated;

                PopWindowManager.ActiveWindows.Remove(displayedTab.Window);
            }

            if (tab != null)
            {
                tab.ShellModeChangeRequested += Tab_ShellModeChangeRequested;

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

        private void Tab_ShellModeChangeRequested(ShellModes mode)
        {
            Mode = mode;
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

        private void TabHistory_HistoryUpdated(TabHistory history)
        {
            buttonPlate.UndoButton.Enabled = history.CanUndo;
            buttonPlate.RedoButton.Enabled = history.CanRedo;
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
