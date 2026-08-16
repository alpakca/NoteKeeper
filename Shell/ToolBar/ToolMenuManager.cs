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

        private bool _movementBoxActive;
        private Point _movementBoxHoldOrigin;

        private readonly ActionToolButton _closeButton;

        private IToolMenu? _shownMenu;
        private IToolButton? _hoveredButton;
        private Rectangle _hoveredButtonBounds;
        private bool _isHoveredButtonHeld;

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
            _isHoveredButtonHeld = false;

            MenuVisibilityChanged?.Invoke(false);
        }

        public void Render()
        {
            if (_shownMenu == null)
                return;

            renderer.Render(_shownMenu);
            renderer.DrawHoveredButtonOverlay(_hoveredButton, _hoveredButtonBounds, _isHoveredButtonHeld);
        }

        public void MousePositionChangeUpdate(PopWindowMouseEventArgs e)
        {
            if (_shownMenu == null)
                return;

            (_hoveredButton, _hoveredButtonBounds) = FindButtonOnPosition(e.MousePosition);

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
                _isHoveredButtonHeld = true;
            }
            else if (_hoveredButton != null && _isHoveredButtonHeld && e.ReleasedButtons.HasLeft)
            {
                _isHoveredButtonHeld = false;
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

        private (IToolButton? Button, Rectangle ButtonBounds) FindButtonOnPosition(Point mousePosition)
        {
            int currentRow = 0;
            int currentColumn = 0;

            foreach (var button in _shownMenu!.Buttons)
            {
                Rectangle layout = layoutCalculator.GetButtonLayout(currentRow, currentColumn);
                if (button.Enabled && layout.Contains(mousePosition))
                {
                    return (button, layout);
                }

                currentColumn++;
                if (ToolMenuLayoutCalculator.ColumnCount == currentColumn)
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }

            Rectangle closeButtonlayout = layoutCalculator.GetCloseButtonLayout();
            if (closeButtonlayout.Contains(mousePosition))
            {
                return (_closeButton, closeButtonlayout);
            }

            return (null, default);
        }
    }
}
