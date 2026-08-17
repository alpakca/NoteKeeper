using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.Shell.Styling;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Text;
using PopUpUI.Toolkit;

namespace NoteKeeper.Shell.ChildWindow
{
    internal class MessageWindow : PopWindow
    {
        private const int MessageTextSize = 12;
        private const int ButtonTextSize = 12;

        private static readonly MessageWindow _window = new MessageWindow();

        private readonly TextPlaque _plaque;
        private readonly WindowButton _closeButton;
        private readonly WindowButton _okButton;

        private WindowButton _hoveredButton;
        private bool _buttonHeld;

        private MessageWindow()
        {
            _plaque = new TextPlaque
            {
                Location = new Point(20, 90),
                Processer = new TextProcesser { FirstRowOffset = 30 },
                Width = 430
            };

            FontUpdated();
            FontProvider.FontChanged += FontUpdated;

            _closeButton = new WindowButton
            {
                BackgroundColor = Color.Transparent,
                Bounds = new Rectangle(445, 0, 55, 38)
            };

            _okButton = new WindowButton
            {
                BackgroundColor = Color.Gray,
                Bounds = new Rectangle(320, 200, 160, 40),
                Text = "Ok"
            };

            Point size = new Point(500, 250);
            this.Boundaries = new Rectangle((WindowOptions.Size - size) / 2, size);
        }

        private void FontUpdated()
        {
            FontProvider.UpdateProcesserFont(_plaque.Processer, MessageTextSize, isBold: false);
        }

        public static void Show(string message)
        {
            _window._plaque.Text = message;

            foreach (var window in PopWindowManager.ActiveWindows)
            {
                window.Frozen = true;
            }

            PopWindowManager.ActiveWindows.Add(_window);
        }

        public static void Remove()
        {
            _window.CloseWindow();
        }

        protected override void RenderUpdate()
        {
            ResolvedTheme theme = ThemeService.CurrentTheme;
            DrawHelper.SetBeginBatch(WindowTexture);

            DrawHelper.DrawBorderedBackground(new Rectangle(default, Boundaries.Size), 
                                              theme.GetColor(ColorRegistry.BorderId), theme.GetColor(ColorRegistry.SurfaceId), 1);

            // close button
            DrawHelper.DrawTextureCenteralized(_closeButton.Bounds, theme.GetIcon(IconRegistry.CloseButtonId));

            // ok button
            RenderButton(_okButton, theme);

            if (_hoveredButton != null)
            {
                DrawHelper.DrawBackground(_hoveredButton.Bounds, _buttonHeld ? theme.GetColor(ColorRegistry.HeldOverlayId)
                                                                             : theme.GetColor(ColorRegistry.HoverOverlayId));
            }

            _plaque.Render();

            DrawHelper.EndBatch();
        }

        private static void RenderButton(WindowButton button, ResolvedTheme theme)
        {
            DrawHelper.DrawBackground(button.Bounds, button.BackgroundColor);

            Vector2 textSize = FontProvider.MeasureSize(ButtonTextSize, isBold: false, button.Text);
            Vector2 textPosition = button.Bounds.Location.ToVector2() + (button.Bounds.Size.ToVector2() - textSize) / 2;

            FontProvider.DrawText(ButtonTextSize, isBold: false, button.Text, textPosition, theme.GetColor(ColorRegistry.TextColorId));
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (e.FirstTimeButtons.HasLeft && _hoveredButton != null && _hoveredButton.Bounds.Contains(e.MousePosition))
            {
                _buttonHeld = true;
            }
            else if (e.ReleasedButtons.HasLeft && _buttonHeld && _hoveredButton != null && _hoveredButton.Bounds.Contains(e.MousePosition))
            {
                _buttonHeld = false;

                // both buttons, close and ok, will close the window
                CloseWindow();
            }
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            if (_closeButton.Bounds.Contains(e.MousePosition))
            {
                _hoveredButton = _closeButton;
            }
            else if (_okButton.Bounds.Contains(e.MousePosition))
            {
                _hoveredButton = _okButton;
            }
            else
            {
                _hoveredButton = null;
            }
        }

        protected override void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            if (key == Keys.Escape)
            {
                CloseWindow();
            }
        }

        private void CloseWindow()
        {
            foreach (var window in PopWindowManager.ActiveWindows)
            {
                window.Frozen = false;
            }

            PopWindowManager.ActiveWindows.Remove(this);
        }
    }
}
