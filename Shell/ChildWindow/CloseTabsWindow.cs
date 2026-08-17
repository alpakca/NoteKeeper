using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell.Styling;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell.ChildWindow
{
    internal class CloseTabsWindow : PopWindow
    {
        private const int HeaderTextSize = 24;
        private const int TabNamesTextSize = 16;
        private const int ButtonTextSize = 12;

        private IEnumerable<Tab> _tabs;

        private readonly List<WindowButton> _buttons;
        private readonly WindowButton _closeButton;

        private WindowButton _hoveredButton;
        private bool _isButtonHeld;

        public IEnumerable<Tab> ClosingTabs => _tabs;
        public event Action<CloseTabsResult> WindowClosed;

        public CloseTabsWindow()
        {
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

            _closeButton = new WindowButton
            {
                BackgroundColor = Color.Transparent,
                Bounds = new Rectangle(505, 0, 55, 38),
                Result = CloseTabsResult.Canceled
            };

            Point size = new Point(560, 400);
            Boundaries = new Rectangle((WindowOptions.Size - size) / 2, size);
        }

        public void SetNew(IEnumerable<Tab> tabs)
        {
            _tabs = tabs;
        }

        protected override void RenderUpdate()
        {
            ResolvedTheme theme = ThemeService.CurrentTheme;
            DrawHelper.SetBeginBatch(WindowTexture);

            DrawHelper.DrawBorderedBackground(new Rectangle(default, Boundaries.Size),
                                              theme.GetColor(ColorRegistry.BorderId), theme.GetColor(ColorRegistry.SurfaceId), 1);

            DrawHelper.DrawTextureCenteralized(_closeButton.Bounds, theme.GetIcon(IconRegistry.CloseButtonId));

            Color textColor = theme.GetColor(ColorRegistry.TextColorId);
            FontProvider.DrawText(HeaderTextSize, isBold: false, "Save changes to following tabs?", new Vector2(10, 20), textColor);

            Vector2 currentOffset = new Vector2(40, 150);
            foreach (var tab in _tabs)
            {
                FontProvider.DrawText(TabNamesTextSize, isBold: false, tab.Name + "*", currentOffset, textColor);
                currentOffset.Y += FontProvider.GetLineSpacing(TabNamesTextSize, isBold: false);
            }

            foreach (var button in _buttons)
            {
                DrawHelper.DrawBackground(button.Bounds, button.BackgroundColor);

                Vector2 textSize = FontProvider.MeasureSize(ButtonTextSize, isBold: false, button.Text);
                Vector2 textPosition = button.Bounds.Location.ToVector2() + (button.Bounds.Size.ToVector2() - textSize) / 2;

                FontProvider.DrawText(ButtonTextSize, isBold: false, button.Text, textPosition, textColor);
            }

            if (_hoveredButton != null)
            {
                DrawHelper.DrawBackground(_hoveredButton.Bounds, _isButtonHeld ? theme.GetColor(ColorRegistry.HeldOverlayId)
                                                                               : theme.GetColor(ColorRegistry.HoverOverlayId));
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

            if (_closeButton.Bounds.Contains(e.MousePosition))
            {
                _hoveredButton = _closeButton;
                return;
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
