using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.Shell.Styling;
using PopUpUI.Elements;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;

namespace NoteKeeper.Shell.ChildWindow
{
    internal class CreateTabFileWindow : PopWindow
    {
        private const int ButtonTextSize = 12;
        private const int HeaderTextSize = 16;
        private const int HorizontalMargin = 20;
        private const int FrameThickness = 4;
        private static readonly CreateTabFileWindow _instance = new CreateTabFileWindow();

        private readonly Clickable _closeButton;
        private readonly LinearTextBox _textBox;

        private readonly ToggleButton[] _optionToggles;
        private readonly Clickable[] _textedButtons;
        private readonly Clickable[] _allButtons;

        private ToggleButton _selectedToggle;

        public event Action<string?> WindowClosed;

        private CreateTabFileWindow() : base(new BatchDrawOptions { BlendState = Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend })
        {
            Point size = new Point(550, 320);
            Boundaries = new Rectangle((WindowOptions.Size - size) / 2, size);

            SelectablePacket selectablePacket = new SelectablePacket();
            HoldablePacket holdablePacket = new HoldablePacket();

            _closeButton = new Clickable
            {
                Boundaries = new Rectangle(Boundaries.Width - 55, 0, 55, 38),
                ClickCommand = new ActionCommander<Clickable>(_ => CloseWindow(additionCancelled: true)),

                SelectablePacket = selectablePacket,
                HoldPacket = holdablePacket
            };
            Clickable cancelButton = new Clickable
            {
                Name = "Cancel",
                Boundaries = new Rectangle(Boundaries.Width - HorizontalMargin - 160, Boundaries.Height - 50, 160, 40),
                ClickCommand = _closeButton.ClickCommand,

                SelectablePacket = selectablePacket,
                HoldPacket = holdablePacket
            };
            Clickable addButton = new Clickable
            {
                Name = "Add",
                Boundaries = new Rectangle(cancelButton.Boundaries.X - HorizontalMargin - 160, cancelButton.Boundaries.Y, 160, 40),
                ClickCommand = new ActionCommander<Clickable>(_ => CloseWindow(additionCancelled: false)),

                SelectablePacket = selectablePacket,
                HoldPacket = holdablePacket
            };

            _textedButtons = [addButton, cancelButton];
            _allButtons = [addButton, cancelButton, _closeButton];

            ToggleButton multiPageTabToggle = new ToggleButton
            {
                Name = "Multi-Paged Note",
                Boundaries = new Rectangle(HorizontalMargin, 70, Boundaries.Width - HorizontalMargin * 2, 40),
                ToggleCommand = new ActionCommander<ToggleButton>(OptionToggled),

                SelectablePacket = selectablePacket,
                HoldPacket = holdablePacket
            };
            multiPageTabToggle.SetStatusWithoutCommander(true);

            _selectedToggle = multiPageTabToggle; // selected by default, first one
            _optionToggles = [multiPageTabToggle];

            int textBoxHeight = FontProvider.GetLineSpacing(ButtonTextSize, isBold: false);
            Rectangle textBoxBounds = new Rectangle(multiPageTabToggle.Boundaries.X, addButton.Boundaries.Y - 20 - textBoxHeight,
                                                    multiPageTabToggle.Boundaries.Width, textBoxHeight);
            _textBox = new LinearTextBox
            {
                TextSize = ButtonTextSize,
                IsTextBold = false,
                ReceivesInput = true,
                Boundaries = textBoxBounds,
                SelectableArea = new Rectangle(textBoxBounds.X - FrameThickness, textBoxBounds.Y - FrameThickness,
                                               textBoxBounds.Width + 2 * FrameThickness, textBoxHeight + 2 * FrameThickness),

                SelectablePacket = selectablePacket
            };

            Elements.AddRange([_closeButton, addButton, cancelButton, multiPageTabToggle, _textBox]);
        }

        public static void Show()
        {
            foreach (var window in PopWindowManager.ActiveWindows) window.Frozen = true;
            PopWindowManager.ActiveWindows.Add(_instance);
        }

        private void OptionToggled(ToggleButton button)
        {
            _selectedToggle = button;
            foreach (var toggle in _optionToggles)
            {
                toggle.SetStatusWithoutCommander(false);
            }

            button.SetStatusWithoutCommander(true);
        }

        private void CloseWindow(bool additionCancelled)
        {
            foreach (var window in PopWindowManager.ActiveWindows) window.Frozen = false;
            PopWindowManager.ActiveWindows.Remove(_instance);

            WindowClosed?.Invoke(additionCancelled ? null : _textBox.Text);
        }

        protected override void RenderUpdate()
        {
            DrawHelper.SetBeginBatch(WindowTexture, DrawOptions);

            ResolvedTheme theme = ThemeService.CurrentTheme;

            DrawHelper.DrawBorderedBackground(new Rectangle(default, Boundaries.Size),
                                              theme.GetColor(ColorRegistry.BorderId), theme.GetColor(ColorRegistry.SurfaceId), 1);

            DrawHelper.DrawTextureCenteralized(_closeButton.Boundaries, theme.GetIcon(IconRegistry.CloseButtonId));

            Color textColor = theme.GetColor(ColorRegistry.TextColorId);
            FontProvider.DrawText(HeaderTextSize, isBold: false, "Select type of file to create", new Vector2(20, 30), textColor);

            foreach (var textedButton in _textedButtons)
            {
                DrawHelper.DrawBackground(textedButton.Boundaries, theme.GetColor(ColorRegistry.BackgroundId));
                FontProvider.DrawCenteredText(ButtonTextSize, isBold: false, textedButton.Name, textedButton.Boundaries, textColor);
            }

            foreach (var toggle in _optionToggles)
            {
                FontProvider.DrawCenteredText(ButtonTextSize, isBold: false, toggle.Name, toggle.Boundaries, textColor);
            }

            foreach (var button in _allButtons)
            {
                if (button.IsHovered)
                {
                    DrawHelper.DrawBackground(button.Boundaries, button.IsHeld ? theme.GetColor(ColorRegistry.HeldOverlayId)
                                                                               : theme.GetColor(ColorRegistry.HoverOverlayId));
                    break;
                }
            }

            // toggled option overlay
            DrawHelper.DrawBackground(_selectedToggle.Boundaries, theme.GetColor(ColorRegistry.HeldOverlayId));

            // draw text box
            DrawHelper.DrawBorderedBackground(_textBox.SelectableArea, theme.GetColor(ColorRegistry.BackgroundId), 
                                              theme.GetColor(ColorRegistry.SurfaceId), 1);

            FontProvider.DrawText(_textBox.TextSize, _textBox.IsTextBold, _textBox.Text,
                                  _textBox.Boundaries.Location.ToVector2(), textColor);

            Point caretOffset = new Point(_textBox.CaretOffset, 0);
            DrawHelper.DrawBackground(new Rectangle(caretOffset + _textBox.Boundaries.Location, new Point(2, _textBox.Boundaries.Height)), textColor);

            DrawHelper.EndBatch();
        }

        protected override void OnKeyDown(PopWindowMouseEventArgs e, Keys key)
        {
            if (key == Keys.Escape)
            {
                CloseWindow(additionCancelled: true);
            }

            base.OnKeyDown(e, key);
        }
    }
}
