using Microsoft.Xna.Framework;
using PopUpUI.Input;
using System;

#nullable enable

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolMenuManager
    {
        private readonly ToolMenuRenderer renderer;
        private readonly ToolMenuLayoutCalculator layoutCalculator;
        private readonly ToolMenuStyleProvider styleProvider;

        private readonly ActionToolButton _closeButton;

        private IToolMenu? _shownMenu;
        private IToolButton? _hoveredButton;
        private bool _isHoveredItemHeld;

        public Point ScreenLocation
        {
            get => layoutCalculator.ScreenLocation;
            set => layoutCalculator.ScreenLocation = value;
        }
        public bool IsShown => _shownMenu != null;

        public event Action<bool>? MenuVisibilityChanged;

        public ToolMenuManager()
        {
            styleProvider = new ToolMenuStyleProvider();

            layoutCalculator = new ToolMenuLayoutCalculator(styleProvider);
            renderer = new ToolMenuRenderer(styleProvider, layoutCalculator);

            _closeButton = new ActionToolButton(Hide);
            styleProvider.ThemeChanged += () =>
            {
                ToolMenuStyle style = styleProvider.Style;
                _closeButton.DefaultTexture = style.CloseButtonTexture;
                _closeButton.HoveredTexture = style.CloseButtonHoverTexture;
                _closeButton.HeldTexture = style.CloseButtonHeldTexture;
            };
        }

        public void Show(IToolMenu menu)
        {
            Hide();

            _shownMenu = menu;
            styleProvider.ApplyTheme(menu.MenuStyle);

            MenuVisibilityChanged?.Invoke(true);
        }

        public void Hide()
        {
            _shownMenu = null;
            _hoveredButton = null;
            _isHoveredItemHeld = false;

            MenuVisibilityChanged?.Invoke(false);
        }

        public void Render()
        {
            if (_shownMenu == null)
                return;

            renderer.Render(_shownMenu, _hoveredButton, _closeButton, _isHoveredItemHeld);
        }

        public void MousePositionChangeUpdate(PopWindowMouseEventArgs e)
        {
            if (_shownMenu == null)
                return;

            _hoveredButton = FindButtonOnPosition(e.MousePosition);
        }

        public void MouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (_shownMenu == null)
                return;

            if (_hoveredButton != null && e.FirstTimeButtons.HasLeft)
            {
                _isHoveredItemHeld = true;
            }
            else if (_hoveredButton != null && _isHoveredItemHeld && e.ReleasedButtons.HasLeft)
            {
                _isHoveredItemHeld = false;
                _hoveredButton.Selected();
            }
        }

        private IToolButton? FindButtonOnPosition(Point mousePosition)
        {
            int currentRow = 0;
            int currentColumn = 0;
            ToolMenuStyle style = styleProvider.Style;

            foreach (var button in _shownMenu!.Buttons)
            {
                if (button.Enabled && layoutCalculator.GetButtonLayout(currentRow, currentColumn).Contains(mousePosition))
                {
                    return button;
                }

                currentColumn++;
                if (style.ColumnCount == currentColumn)
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }

            if (layoutCalculator.GetCloseButtonLayout().Contains(mousePosition))
            {
                return _closeButton;
            }

            return null;
        }
    }
}
