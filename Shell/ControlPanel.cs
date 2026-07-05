using NoteKeeper.InspectionWindow;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace NoteKeeper.Shell
{
    internal class ControlPanel
    {
        private Tab displayedTab;

        private readonly PanelWindow window;
        private readonly List<ITabUpdateReceiver> tabUpdateReceivers;

        public ShellModes Mode
        {
            get;
            set
            {
                field = value;
                displayedTab.ShellModeUpdated(value);
            }
        }
        public Tab DisplayedTab => displayedTab;
        public List<Tab> ActiveTabs { get; }

        public ControlPanel()
        {
            ActiveTabs = new List<Tab>();
            tabUpdateReceivers = new List<ITabUpdateReceiver>();

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

            ButtonPlate buttonPlate = new ButtonPlate(this);

            window.Elements.Add(verticalScroller);
            window.Elements.Add(horizontalScroller);
            window.Elements.AddRange(buttonPlate.Buttons);

            tabUpdateReceivers.AddRange(verticalScroller, horizontalScroller, window);
        }

        /// <summary>Rearranges heights of TabGroup, ButtonPlate and DisplayedTab; by chaning DisplayedTab's height and keeping others minimum.</summary>
        public void RearrangeUISizes()
        {

        }

        public void DisplayTab(Tab tab)
        {
            if (!ActiveTabs.Contains(tab))
            {
                return;
            }

            PopWindowManager.ActiveWindows.Remove(displayedTab?.Window);

            displayedTab = tab;

            foreach (var receiver in tabUpdateReceivers)
            {
                receiver.DisplayedTabChanged(tab);
            }

            PopWindowManager.ActiveWindows.Add(tab.Window);
            PopWindowManager.ActiveWindows.MoveToBottom(tab.Window);
        }
    }
}
