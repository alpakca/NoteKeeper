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
        private static readonly MessageWindow _window = new MessageWindow();

        private readonly TextPlaque _plaque;
        private readonly WindowButton _closeButton;

        private bool _buttonHeld;

        private MessageWindow()
        {
            _plaque = new TextPlaque
            {
                Location = new Point(20, 90),
                Processer = new TextProcesser
                {
                    Font = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_16"),
                    FirstRowOffset = 30
                },
                Width = 430
            };

            _closeButton = new WindowButton
            {
                BackgroundColor = Color.Red,
                Bounds = new Rectangle(450, 0, 50, 35)
            };

            Point size = new Point(500, 200);
            this.Boundaries = new Rectangle((WindowOptions.Size - size) / 2, size);
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
            DrawHelper.SetBeginBatch(WindowTexture);

            DrawHelper.DrawBackground(new Rectangle(default, this.Boundaries.Size), Color.SlateGray);
            DrawHelper.DrawBackground(_closeButton.Bounds, _closeButton.BackgroundColor);

            _plaque.Render();

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
