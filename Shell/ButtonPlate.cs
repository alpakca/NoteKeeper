using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow;
using PopUpUI.Elements;
using PopUpUI.Elements.Visual;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell
{
    internal class ButtonPlate
    {
        private readonly ControlPanel panel;

        public List<Clickable> Buttons { get; }
        public int MinimumVisualHeight { get; }

        public ButtonPlate(ControlPanel panel)
        {
            this.panel = panel;

            Buttons = new List<Clickable>();
            InitializeButtons();
        }

        private void InitializeButtons()
        {
            SelectablePacket selectablePacket = new SelectablePacket();
            HoldablePacket holdPacket = new HoldablePacket();

            Buttons.Add(new VisualToggleButton
            {
                Name = "Writing Mode Toggle",

                Boundaries = new Rectangle(100, 60, 32, 32),
                UncheckedAnimation = Animation2D.FromTexture(ContentAssistant.FromLoaded<Texture2D>("Button_WritingMode")),
                CheckedAnimation = Animation2D.FromTexture(ContentAssistant.FromLoaded<Texture2D>("Button_WritingMode_Selected")),

                SelectablePacket = selectablePacket,
                HoldPacket = holdPacket
            });

            Buttons.Add(new VisualToggleButton
            {
                Name = "Transform Mode Toggle",

                Boundaries = new Rectangle(130, 60, 32, 32),
                UncheckedAnimation = Animation2D.FromTexture(ContentAssistant.FromLoaded<Texture2D>("Button_TransformMode")),
                CheckedAnimation = Animation2D.FromTexture(ContentAssistant.FromLoaded<Texture2D>("Button_TransformMode_Selected")),

                SelectablePacket = selectablePacket,
                HoldPacket = holdPacket
            });

            (Buttons[0] as ToggleButton).ToggleCommand = new OppositeToggleButtonsCommand(Buttons[1] as ToggleButton, () => panel.Mode = ShellModes.Writing);
            (Buttons[1] as ToggleButton).ToggleCommand = new OppositeToggleButtonsCommand(Buttons[0] as ToggleButton, () => panel.Mode = ShellModes.Transform);

            Buttons.Add(new VisualClickable
            {
                Name = "Text Content Add Button",

                Boundaries = new Rectangle(160, 60, 32, 32),
                DefaultAnimation = Animation2D.FromTexture(ContentAssistant.FromLoaded<Texture2D>("Button_AddTextBlock")),
                HoverAnimation = Animation2D.FromTexture(ContentAssistant.FromLoaded<Texture2D>("Button_AddTextBlock_Hovered")),
                HoldAnimation = Animation2D.FromTexture(ContentAssistant.FromLoaded<Texture2D>("Button_AddTextBlock_Held")),

                SelectablePacket = selectablePacket,
                HoldPacket = holdPacket,

                ClickCommand = new ActionCommander<Clickable>(_ =>
                {
                    if (panel.DisplayedTab == null)
                    {
                        return;
                    }

                    PageTextContent content = ContentGetter.GetTextContent();
                    content.Location = new Point(100, 100);
                    content.Size = new Point(200, 50);

                    panel.DisplayedTab.AddContent(content);
                    ActivateWritingMode();

                    panel.DisplayedTab.SimulateMouseButtonStateInput(new PopWindowMouseEventArgs(MouseButtons.Left,
                                                                                                 MouseButtons.Left,
                                                                                                 MouseButtons.None,
                                                                                                 panel.DisplayedTab.GetContentScreenLocation(content)));
                })
            });

            ActivateWritingMode();
        }

        public void ActivateWritingMode()
        {
            (Buttons[0] as ToggleButton).Toggle();
        }

        public void ActivateTransformMode()
        {
            (Buttons[1] as ToggleButton).Toggle();
        }

        private class OppositeToggleButtonsCommand : ICommander<ToggleButton>
        {
            private readonly Action action;
            private readonly ToggleButton oppositeButton;

            public OppositeToggleButtonsCommand(ToggleButton oppositeButton, Action action)
            {
                this.action = action;
                this.oppositeButton = oppositeButton;
            }

            public void Execute(ToggleButton thisButton)
            {
                // This button was toggled even though it was checked
                if (!oppositeButton.IsChecked && !thisButton.IsChecked)
                {
                    thisButton.Toggle();
                    return;
                }

                if (thisButton.IsChecked)
                {
                    action();

                    if (oppositeButton.IsChecked)
                    {
                        oppositeButton.Toggle();
                    }
                }
            }
        }
    }
}
