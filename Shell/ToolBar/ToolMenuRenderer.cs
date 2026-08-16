using Microsoft.Xna.Framework;
using NoteKeeper.Shell.Styling;
using PopUpUI.Toolkit;

namespace NoteKeeper.Shell.ToolBar
{
    internal class ToolMenuRenderer
    {
        private readonly ToolMenuLayoutCalculator _layoutCalculator;

        public ToolMenuRenderer(ToolMenuLayoutCalculator layoutCalculator)
        {
            _layoutCalculator = layoutCalculator;
        }

        public void Render(IToolMenu menu, IToolButton hoveredButton, IToolButton closeButton, bool isHeld)
        {
            int currentRow = 0;
            int currentColumn = 0;
            ResolvedTheme theme = ThemeService.CurrentTheme;

            DrawHelper.DrawBorderedBackground(new Rectangle(_layoutCalculator.ScreenLocation, _layoutCalculator.GetBackgroundSize(menu.Buttons.Length)),
                                              theme.GetColor(ToolMenuStyles.Border), theme.GetColor(ToolMenuStyles.Background), 1);

            DrawHelper.Batch.Draw(theme.GetIcon(ToolMenuStyles.MovementBox), _layoutCalculator.GetMoveBoxLayout(), Color.White);
            DrawHelper.Batch.Draw(theme.GetIcon(ToolMenuStyles.CloseButton), _layoutCalculator.GetCloseButtonLayout(), Color.White);

            foreach (var button in menu.Buttons)
            {
                Rectangle layout = _layoutCalculator.GetButtonLayout(currentRow, currentColumn);
                DrawHelper.Batch.Draw(button.Texture, layout, Color.White);
                
                currentColumn++;
                if (currentColumn == ToolMenuLayoutCalculator.ColumnCount)
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }
        }

        public void DrawHoveredButtonOverlay(Rectangle bounds, bool isHeld)
        {
            DrawHelper.DrawBackground(bounds, isHeld ? ThemeService.CurrentTheme.GetColor(ToolMenuStyles.HeldOverlay)
                                                     : ThemeService.CurrentTheme.GetColor(ToolMenuStyles.HoverOverlay));
        }
    }
}
