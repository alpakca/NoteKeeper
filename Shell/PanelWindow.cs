using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.ContextMenus;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class PanelWindow : PopWindow, ITabUpdateReceiver
    {
        private Tab? displayedTab;

        private readonly ControlPanel panel;
        private readonly ContextMenuManager contextMenuManager;
        private readonly KeyboardShortcutManager shortcutManager;

        public PanelWindow(ControlPanel panel, Point size) : base(size)
        {
            this.panel = panel;

            contextMenuManager = new ContextMenuManager();
            shortcutManager = new KeyboardShortcutManager();

            Elements.Add(shortcutManager);
            InitializeShortcuts();
        }

        private void InitializeShortcuts()
        {
            KeyboardCombination zoomIn = KeyboardCombination.GetSingleUse([Keys.LeftControl, Keys.Add]);
            zoomIn.FirstTriggerAction = () => displayedTab?.TabViewport.Zoom += 0.1f;

            KeyboardCombination zoomOut = KeyboardCombination.GetSingleUse([Keys.LeftControl, Keys.Subtract]);
            zoomOut.FirstTriggerAction = () => displayedTab?.TabViewport.Zoom -= 0.1f;

            shortcutManager.AddCombinations(zoomIn, zoomOut);
        }

        public void DisplayedTabChanged(Tab setTab)
        {
            displayedTab = setTab;
        }

        public void ZoomChanged()
        {
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

            // render context menu on top of everything
            contextMenuManager.RenderUpdate(WindowTexture);
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (e.ReleasedButtons.HasRight)
            {
                ContextMenu? contextMenu;
                if (displayedTab != null && displayedTab.Window.Boundaries.Contains(e.MousePosition))
                {
                    contextMenu = displayedTab.GetContextMenu(e.MousePosition - displayedTab.Window.Boundaries.Location);
                }
                else
                {
                    contextMenu = panel.GetContextMenu(e.MousePosition);
                }

                if (contextMenu != null)
                {
                    contextMenuManager.Show(contextMenu);
                }
            }

            contextMenuManager.MouseButtonStateUpdate(e);
            base.OnMouseButtonStateChange(e);
        }
    }
}
