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

        private readonly ActionToolButton _closeButton;

        private bool _movementBoxActive;
        private Point _movementBoxHoldOrigin;

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
            layoutCalculator = new ToolMenuLayoutCalculator();
            renderer = new ToolMenuRenderer(layoutCalculator);

            _closeButton = new ActionToolButton(Hide);
        }

        public void Show(IToolMenu menu)
        {
            Hide();

            _shownMenu = menu;

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

            if (_movementBoxActive)
            {
                ScreenLocation = e.MousePosition - _movementBoxHoldOrigin;
            }
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
            
            // if they there any buttons (including close button) that is hovered, don't let movement of menu
            if (_hoveredButton == null && layoutCalculator.GetMoveBoxLayout().Contains(e.MousePosition))
            {
                if (e.FirstTimeButtons.HasLeft)
                {
                    _movementBoxHoldOrigin = e.MousePosition - ScreenLocation;
                    _movementBoxActive = true;

                }
                else if (e.ReleasedButtons.HasLeft)
                {
                    ScreenLocation = e.MousePosition - _movementBoxHoldOrigin;
                    _movementBoxActive = false;
                }
            }
        }

        private IToolButton? FindButtonOnPosition(Point mousePosition)
        {
            int currentRow = 0;
            int currentColumn = 0;

            foreach (var button in _shownMenu!.Buttons)
            {
                if (button.Enabled && layoutCalculator.GetButtonLayout(currentRow, currentColumn).Contains(mousePosition))
                {
                    return button;
                }

                currentColumn++;
                if (ToolMenuLayoutCalculator.ColumnCount == currentColumn)
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
