using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Input;
using NoteKeeper.Project;
using NoteKeeper.Shell.Styling;
using PopUpUI.Elements;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell.ChildWindow
{
    internal delegate void CreateTabFileWindowClosed(string? filePath, string tabExtension);

    internal class CreateTabFileWindow : PopWindow
    {
        private const int ButtonTextSize = 12;
        private const int HeaderTextSize = 16;
        private const int HorizontalMargin = 20;
        private const int FrameThickness = 4;
        private static readonly CreateTabFileWindow _instance = new CreateTabFileWindow();

        private readonly Clickable _closeButton;
        private readonly LinearTextBox _textBox;

        private readonly List<TabToggleButton> _optionToggles;
        private readonly Clickable[] _textedButtons;
        private readonly Clickable[] _allButtons;

        private TabToggleButton _selectedToggle;

        public static event CreateTabFileWindowClosed WindowClosed;

        private CreateTabFileWindow() : base(new BatchDrawOptions { BlendState = Microsoft.Xna.Framework.Graphics.BlendState.AlphaBlend })
        {
            SelectablePacket selectablePacket = new SelectablePacket();
            HoldablePacket holdablePacket = new HoldablePacket();

            const int ToggleButtonHeight = 40;
            const int WindowWidth = 550;

            int yOffset = 70;
            ActionCommander<ToggleButton> toggleAction = new ActionCommander<ToggleButton>(OptionToggled);
            _optionToggles = new List<TabToggleButton>();

            foreach (var tabInfo in TabService.TabTypes)
            {
                TabToggleButton tabToggle = new TabToggleButton
                {
                    Name = tabInfo.FileTypeName,
                    FileExtension = tabInfo.Extension,

                    Boundaries = new Rectangle(HorizontalMargin, yOffset, WindowWidth - HorizontalMargin * 2, ToggleButtonHeight),
                    ToggleCommand = toggleAction,

                    SelectablePacket = selectablePacket,
                    HoldPacket = holdablePacket
                };

                yOffset += ToggleButtonHeight;
                _optionToggles.Add(tabToggle);
            }

            _selectedToggle = _optionToggles[0]; // selected by default, first one
            _selectedToggle.SetStatusWithoutCommander(true);

            int textBoxHeight = FontProvider.GetLineSpacing(ButtonTextSize, isBold: false);
            Rectangle lastTabToggleBounds = _optionToggles[^1].Boundaries;
            Rectangle textBoxBounds = new Rectangle(lastTabToggleBounds.X, yOffset, lastTabToggleBounds.Width, textBoxHeight);
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

            yOffset += textBoxHeight + ToggleButtonHeight;
            const int BottomButtonHeight = 40;
            const int BottomMargin = 10;

            Point size = new Point(WindowWidth, yOffset + BottomButtonHeight + BottomMargin);
            Boundaries = new Rectangle((WindowOptions.Size - size) / 2, size);

            _closeButton = new Clickable
            {
                Boundaries = new Rectangle(WindowWidth - 55, 0, 55, 38),
                ClickCommand = new ActionCommander<Clickable>(_ => CloseWindow(additionCancelled: true)),

                SelectablePacket = selectablePacket,
                HoldPacket = holdablePacket
            };
            Clickable cancelButton = new Clickable
            {
                Name = "Cancel",
                Boundaries = new Rectangle(WindowWidth - HorizontalMargin - 160, Boundaries.Height - 50, 160, 40),
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

            Elements.AddRange([_closeButton, addButton, cancelButton, .._optionToggles, _textBox]);
        }

        public static void Show(string examplePath)
        {
            foreach (var window in PopWindowManager.ActiveWindows) window.Frozen = true;
            PopWindowManager.ActiveWindows.Add(_instance);

            _instance._textBox.Text = examplePath;
        }

        private void OptionToggled(ToggleButton button)
        {
            _selectedToggle = (TabToggleButton)button;
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

            WindowClosed?.Invoke(additionCancelled ? null : _textBox.Text, _selectedToggle.FileExtension);
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

        private class TabToggleButton : ToggleButton
        {
            public string FileExtension { get; init; }
        }
    }
}
