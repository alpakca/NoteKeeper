using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;
using PopUpUI.Input;
using Microsoft.Xna.Framework.Input;

namespace NoteKeeper.Shell.UnsavedTabsWindow
{
    internal class CloseTabsWindow : PopWindow
    {
        private IEnumerable<Tab> _tabs;

        private readonly SpriteFont _topTextFont;
        private readonly SpriteFont _tabNamesFont;

        private readonly List<WindowButton> _buttons;
        private WindowButton _hoveredButton;
        private bool _isButtonHeld;

        public IEnumerable<Tab> ClosingTabs => _tabs;
        public event Action<CloseTabsResult> WindowClosed;

        public CloseTabsWindow()
        {
            _topTextFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_24");
            _tabNamesFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans");

            _buttons = [
            new WindowButton {
                BackgroundColor = Color.Purple,
                Bounds = new Rectangle(20, 350, 160, 40),
                Result = CloseTabsResult.Save,
                Text = "Save"
            },
            new WindowButton {
                BackgroundColor = Color.Gray,
                Bounds = new Rectangle(200, 350, 160, 40),
                Result = CloseTabsResult.DontSave,
                Text = "Don't Save"
            },
            new WindowButton {
                BackgroundColor = Color.Gray,
                Bounds = new Rectangle(380, 350, 160, 40),
                Result = CloseTabsResult.Canceled,
                Text = "Cancel"
            }];

            Point size = new Point(560, 400);
            Boundaries = new Rectangle((WindowOptions.Size - size) / 2, size);
        }

        public void SetNew(IEnumerable<Tab> tabs)
        {
            _tabs = tabs;
        }

        protected override void RenderUpdate()
        {
            DrawHelper.SetBeginBatch(WindowTexture);

            DrawHelper.DrawBackground(new Rectangle(new Point(), this.Boundaries.Size), Color.Black);
            DrawHelper.Batch.DrawString(_topTextFont, "Save changes to following tabs?", new Vector2(10, 20), Color.White);

            Vector2 currentOffset = new Vector2(40, 150);
            foreach (var tab in _tabs)
            {
                DrawHelper.Batch.DrawString(_tabNamesFont, tab.Name + "*", currentOffset, Color.White);
                currentOffset.Y += _tabNamesFont.LineSpacing;
            }

            foreach (var button in _buttons)
            {
                DrawHelper.DrawBackground(button.Bounds, button.BackgroundColor);

                Vector2 textSize = _tabNamesFont.MeasureString(button.Text);
                Vector2 textPosition = button.Bounds.Location.ToVector2() + (button.Bounds.Size.ToVector2() - textSize) / 2;
                DrawHelper.Batch.DrawString(_tabNamesFont, button.Text, textPosition, Color.White);
            }

            if (_hoveredButton != null)
            {
                DrawHelper.DrawBackground(_hoveredButton.Bounds, _isButtonHeld ? Color.Gray * 0.5f : Color.LightGray * 0.7f);
            }

            DrawHelper.EndBatch();
        }

        protected override void OnMouseButtonStateChange(PopWindowMouseEventArgs e)
        {
            if (e.FirstTimeButtons.HasLeft && _hoveredButton != null && _hoveredButton.Bounds.Contains(e.MousePosition))
            {
                _isButtonHeld = true;
            }
            else if (e.ReleasedButtons.HasLeft && _isButtonHeld && _hoveredButton != null && _hoveredButton.Bounds.Contains(e.MousePosition))
            {
                _isButtonHeld = false;
                WindowClosed?.Invoke(_hoveredButton.Result);
            }
        }

        protected override void OnMousePositionChange(PopWindowMouseEventArgs e)
        {
            foreach (var button in _buttons)
            {
                if (button.Bounds.Contains(e.MousePosition))
                {
                    _hoveredButton = button;
                    return;
                }
            }

            _hoveredButton = null;
        }

        protected override void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            if (key == Keys.Escape)
            {
                WindowClosed?.Invoke(CloseTabsResult.Canceled);
            }
        }
    }
}
