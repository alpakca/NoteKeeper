using NoteKeeper.InspectionWindow;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System.Collections.Generic;
using Microsoft.Xna.Framework;
using System.Collections.ObjectModel;

namespace NoteKeeper.Shell
{
    internal class ControlPanel
    {
        private Tab displayedTab;

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
            }
        }
        public Tab DisplayedTab => displayedTab;
        public ObservableCollection<Tab> ActiveTabs { get; }

        public ControlPanel()
        {
            tabUpdateReceivers = new List<ITabUpdateReceiver>();
            ActiveTabs = new ObservableCollection<Tab>();

            buttonPlate = new ButtonPlate(this);
            tabGroup = new TabGroup(this) { Boundaries = new Rectangle(20, 170, 1720, 30) };

            window = new PanelWindow(WindowOptions.Size);
            InitializeWindow();

            PopWindowManager.ActiveWindows.Add(window);
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

        /// <summary>Rearranges heights of TabGroup, ButtonPlate and DisplayedTab; by chaning DisplayedTab's height and keeping others minimum.</summary>
        public void RearrangeUISizes()
        {

        }

        public void DisplayTab(Tab tab)
        {
            if (displayedTab != null)
            {
                displayedTab.TabViewport.ZoomChanged -= TabViewport_ZoomChanged;
                PopWindowManager.ActiveWindows.Remove(displayedTab.Window);
            }

            displayedTab = tab;
            tab.ShellModeUpdated(Mode);

            foreach (var receiver in tabUpdateReceivers)
            {
                receiver.DisplayedTabChanged(tab);
            }

            if (tab != null)
            {
                tab.TabViewport.ZoomChanged += TabViewport_ZoomChanged;

                PopWindowManager.ActiveWindows.Add(tab.Window);
                PopWindowManager.ActiveWindows.MoveToBottom(tab.Window);
            }

            RearrangeUISizes();
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
