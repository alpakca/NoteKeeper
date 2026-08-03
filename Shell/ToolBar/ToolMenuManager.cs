using Microsoft.Xna.Framework;
using PopUpUI.Input;

#nullable enable

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolMenuManager
    {
        private readonly ToolMenuRenderer renderer;
        private readonly ToolMenuLayoutCalculator layoutCalculator;

        private IToolMenu? _shownMenu;
        private ToolButton? _hoveredButton;
        private bool _isHoveredItemHeld;

        public Point ScreenLocation
        {
            get => layoutCalculator.ScreenLocation;
            set => layoutCalculator.ScreenLocation = value;
        }
        public ToolMenuStyleProvider StyleProvider { get; }

        public ToolMenuManager()
        {
            StyleProvider = new ToolMenuStyleProvider();

            layoutCalculator = new ToolMenuLayoutCalculator(StyleProvider);
            renderer = new ToolMenuRenderer(StyleProvider, layoutCalculator);
        }

        public void Show(IToolMenu menu)
        {
            Hide();

            _shownMenu = menu;
        }

        public void Hide()
        {
            _shownMenu = null;
            _hoveredButton = null;
            _isHoveredItemHeld = false;
        }

        public void Render()
        {
            if (_shownMenu == null)
                return;

            renderer.Render(_shownMenu, _hoveredButton, _isHoveredItemHeld);
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

        private ToolButton? FindButtonOnPosition(Point mousePosition)
        {
            int currentRow = 0;
            int currentColumn = 0;
            ToolMenuStyle style = StyleProvider.Style;

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

            return null;
        }
    }
}
