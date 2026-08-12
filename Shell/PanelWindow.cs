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
        private readonly PropertiesScreen propertiesScreen;
        private readonly HistoryActionsManager historyActionsManager;
        private readonly KeyboardShortcutManager shortcutManager;

        public ToolMenuManager ToolMenuManager => toolMenuManager;
        public PropertiesScreen PropertiesScreen => propertiesScreen;
        public HistoryActionsManager HistoryActionsManager => historyActionsManager;

        public PanelWindow(ControlPanel panel)
        {
            this.panel = panel;

            contextMenuManager = new ContextMenuManager();
            toolMenuManager = new ToolMenuManager() { ScreenLocation = new Point(60, 240) };
            tabOrganizeManager = new TabOrganizeManager(panel.ActiveTabs) { ScreenBounds = new Rectangle(20, 168, 1880, 32) };
            propertiesScreen = new PropertiesScreen();
            historyActionsManager = new HistoryActionsManager();
            shortcutManager = new KeyboardShortcutManager();

            tabOrganizeManager.DisplayedTabChanged += panel.DisplayTab;
            historyActionsManager.ActionsReversed += HistoryActionsManager_ActionsReversed;

            #region Debug Styles
            tabOrganizeManager.StyleProvider.ApplyTheme(new TabsHeaderStyle
            {
                TabDefaultBackground = Util.GetTexture(1, 1, Color.Gray),
                TabHoveredBackground = Util.GetTexture(1, 1, Color.LightGray),
                DisplayedTabBackground = Util.GetTexture(1, 1, Color.DarkMagenta),

                DefaultFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_16"),
                DisplayedTabFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_16"),

                TabCloseButtonIcon = Util.GetTexture(1, 1, Color.Red),
                TabListButtonIcon = Util.GetTexture(1, 1, Color.Aqua),
                ButtonHoverOverlay = Util.GetTexture(1, 1, Color.Gray * 0.5f),
                ButtonHeldOverlay = Util.GetTexture(1, 1, Color.LightGray * 0.5f),

                HeaderBackground = Util.GetTexture(1, 1, Color.Gray)
            });

            MenuButtonStyle buttonStyle = new MenuButtonStyle
            {
                HorizontalPadding = 2,
                IconHorizontalPadding = 2,
                LeftIconSize = new Point(25),
                LeftRightPartPadding = 20,
                LeftTextColor = Color.White,
            };

            contextMenuManager.StyleProvider.ApplyTheme(new ContextMenuStyle
            {
                Background = Util.GetTexture(1, 1, Color.SlateGray),
                HoverOverlay = Util.GetTexture(1, 1, Color.LightGray * 0.3f),
                HeldOverlay = Util.GetTexture(1, 1, Color.DarkGray * 0.3f),
                Font = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_16"),

                ItemStyles = new Dictionary<Type, MenuItemStyle>
                {
                    { typeof(ActionContextMenuButton), new ActionMenuItemStyle
                        {
                            ButtonStyle = buttonStyle,
                            RightTextColor = Color.WhiteSmoke,
                            Height = 32,
                        } 
                    },
                    {
                        typeof(SubContextMenuButton), new SubContextMenuItemStyle
                        {
                            ButtonStyle = buttonStyle,
                            RightIconSize = new Point(25),
                            Height = 32
                        }
                    },
                    {
                        typeof(GroupLineItem), new GroupLineStyle
                        {
                            LineColor = Color.WhiteSmoke,
                            Thickness = 2,
                            Height = 4
                        }
                    }
                }
            });

            propertiesScreen.StyleProvider.ApplyTheme(new PropertyScreenStyle
            {
                Font = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_16"),
                BoxBackground = Util.GetTexture(1, 1, Color.Black),
                DropdownButtonTexture = Util.GetTexture(1, 1, Color.Gray),
                ScreenBackground = Util.GetTexture(1, 1, Color.DarkGray),
                DropdownOptionBackground = Util.GetTexture(1, 1, Color.LightGray),
                FocusedBoxBackground = Util.GetTexture(1, 1, Color.Purple)
            });

            historyActionsManager.StyleProvider.ApplyTheme(new HistoryActionsStyle
            {
                Font = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_16"),
                Background = Util.GetTexture(1, 1, Color.SlateGray),
                Height = 24,
                SelectedActionsHover = Util.GetTexture(1, 1, Color.LightBlue * 0.3f),
                SelectedActionsIcon = Util.GetTexture(1, 1, Color.Purple),
                SelectedActionsIconSize = new Point(4, 20),
                TextHorizontalPadding = 16
            });
            #endregion

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

            tabOrganizeManager.DisplayedTabUpdated(setTab);

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

            DrawHelper.SetBeginBatch(WindowTexture);

            toolMenuManager.Render();

            if (panel.ActiveTabs.Count != 0)
            {
                tabOrganizeManager.Render();
            }

            propertiesScreen.Render();
            historyActionsManager.Render();

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
