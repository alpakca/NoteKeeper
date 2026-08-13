using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Text;
using PopUpUI.Toolkit;
using System;

namespace NoteKeeper.Shell.ChildWindow
{
    internal class MessageWindow : PopWindow
    {
        private static MessageWindow _window = new MessageWindow();

        private readonly TextProcesser processer;
        private ITextProcesserResult _lastResult;
        private readonly WindowButton _closeButton;

        private bool _buttonHeld;

        private MessageWindow()
        {
            processer = new TextProcesser
            {
                Font = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_16"),
                FirstRowOffset = 20
            };

            _closeButton = new WindowButton
            {
                BackgroundColor = Color.Red,
                Bounds = new Rectangle(450, 0, 50, 35)
            };

            Point size = new Point(500, 200);
            this.Boundaries = new Rectangle((WindowOptions.Size - size) / 2, size);

            processer.HorizontalTextLength = size.X;
        }

        public static void Show(string message)
        {
            _window.processer.Text = message;
            _window._lastResult = _window.processer.GetParseResult();

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
            DrawHelper.SetBeginBatch(WindowTexture);

            DrawHelper.DrawBackground(new Rectangle(default, this.Boundaries.Size), Color.SlateGray);
            DrawHelper.DrawBackground(_closeButton.Bounds, _closeButton.BackgroundColor);

            _lastResult.Render(BatchDrawOptions.Default, new Vector2(20, 90), null, 1f);

            DrawHelper.EndBatch();
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (e.FirstTimeButtons.HasLeft && _closeButton.Bounds.Contains(e.MousePosition))
            {
                _buttonHeld = true;
            }
            else if (e.ReleasedButtons.HasLeft && _buttonHeld && _closeButton.Bounds.Contains(e.MousePosition))
            {
                _buttonHeld = false;
                CloseWindow();
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
