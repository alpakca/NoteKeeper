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

        public void Render(IToolMenu menu, IToolButton hoveredButton, bool isHeld)
        {
            int currentRow = 0;
            int currentColumn = 0;
            ResolvedTheme theme = ThemeService.CurrentTheme;

            DrawHelper.DrawBorderedBackground(new Rectangle(_layoutCalculator.ScreenLocation, _layoutCalculator.GetBackgroundSize(menu.Buttons.Length)),
                                              theme.GetColor(ToolMenuStyles.Border), theme.GetColor(ToolMenuStyles.Background), 1);

            DrawHelper.Batch.Draw(theme.GetIcon(ToolMenuStyles.CloseButton), _layoutCalculator.GetCloseButtonLayout(), Color.White);

            foreach (var button in menu.Buttons)
            {
                Rectangle layout = _layoutCalculator.GetButtonLayout(currentRow, currentColumn);

                DrawHelper.Batch.Draw(button.Enabled ? button.DefaultTexture : button.UnenabledTexture, layout, Color.White);
                DrawButtonOverlay(button, layout, isHovered: button == hoveredButton, isHeld);
                
                currentColumn++;
                if (currentColumn == ToolMenuLayoutCalculator.ColumnCount)
                {
                    currentColumn = 0;
                    currentRow++;
                }
            }
        }

        private void DrawButtonOverlay(IToolButton button, Rectangle bounds, bool isHovered, bool isHeld)
        {
            if (button is ActionToolButton && isHovered)
            {
                DrawHelper.DrawBackground(bounds, isHeld ? ThemeService.CurrentTheme.GetColor(ToolMenuStyles.HeldOverlay)
                                                         : ThemeService.CurrentTheme.GetColor(ToolMenuStyles.HoverOverlay));
            }
            else if (button is ToggleToolButton toggle && toggle.IsChecked)
            {
                // toggle buttons always overlayed with held if checked
                DrawHelper.DrawBackground(bounds, ThemeService.CurrentTheme.GetColor(ToolMenuStyles.HeldOverlay));
            }
        }
    }
}
