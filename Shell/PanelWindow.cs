using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.ContextMenus;
using NoteKeeper.Shell.ToolBar;
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
        private readonly ToolMenuManager toolMenuManager;
        private readonly KeyboardShortcutManager shortcutManager;

        public PanelWindow(ControlPanel panel, Point size) : base(size)
        {
            this.panel = panel;

            contextMenuManager = new ContextMenuManager();
            toolMenuManager = new ToolMenuManager() { ScreenLocation = new Point(60, 240) };
            shortcutManager = new KeyboardShortcutManager();

            MenuItemStyle actionButtonStyle = new MenuItemStyle
            {
                Background = Util.GetTexture(1, 1, Color.DarkRed),
                BoundaryPadding = new Point(4),
                IconHorizontalPadding = 4,
                IconSize = new Point(32),
                LeftText = "Action!",
                LeftTextColor = Color.DarkTurquoise,
                LeftTextFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans"),
                LeftIcon = Util.GetTexture(1, 1, Color.DarkSlateBlue),
                HeldOverlay = Util.GetTexture(1, 1, Color.Firebrick),
                HoverOverlay = Util.GetTexture(1, 1, Color.DimGray)
            };

            MenuItemStyle subMenuStyle = new MenuItemStyle
            {
                Background = Util.GetTexture(1, 1, Color.DarkRed),
                BoundaryPadding = new Point(4),
                IconHorizontalPadding = 4,
                IconSize = new Point(32),
                LeftText = "Action!",
                LeftTextColor = Color.DarkTurquoise,
                LeftTextFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans"),
                LeftIcon = Util.GetTexture(1, 1, Color.DarkSlateBlue)
            };

            Dictionary<Type, MenuItemStyle> defaultTheme = new Dictionary<Type, MenuItemStyle>
            {
                { typeof(ActionContextMenuButton), actionButtonStyle }, { typeof(SubContextMenuButton), subMenuStyle }
            };
            contextMenuManager.StyleProvider.ApplyTheme(defaultTheme);

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

        public void DisplayedTabChanged(Tab? setTab)
        {
            displayedTab = setTab;

            // switched the open tool menu with new tab's tool menu
            SetToolMenuVisibility(toolMenuManager.IsShown);
        }

        public void SetToolMenuVisibility(bool isVisible)
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

            DrawHelper.SetBeginBatch(WindowTexture);
            toolMenuManager.Render();
            DrawHelper.EndBatch();

            // render context menu on top of everything
            contextMenuManager.RenderUpdate(WindowTexture);
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
            base.OnMouseButtonStateChange(e);
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            toolMenuManager.MousePositionChangeUpdate(e);
            contextMenuManager.MousePositionChangeUpdate(e);
            base.OnMousePositionChange(e);
        }
    }
}
