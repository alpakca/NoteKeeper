using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using NoteKeeper.InspectionWindow.PagedTabs;
using NoteKeeper.Shell.PUIHelpers;
using PopUpUI.Elements;
using PopUpUI.Elements.Visual;
using PopUpUI.Input;
using PopUpUI.PUI;
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
            PUIInterpreter interpreter = PUIHelper.GetInterpreter(PUIHelper.ShellAssetsFolderPath + "ButtonPlate.pui");
            interpreter.ProcessTokens();

            VisualToggleButton writingModeToggle = interpreter.CreateFromProperties(PUIHelper.VisualToggleButtonInstantiator, "Writing_Mode_Toggle");
            VisualToggleButton transformModeToggle = interpreter.CreateFromProperties(PUIHelper.VisualToggleButtonInstantiator, "Transform_Mode_Toggle");

            writingModeToggle.ToggleCommand = new OppositeToggleButtonsCommand(transformModeToggle, () => panel.Mode = ShellModes.Writing);
            transformModeToggle.ToggleCommand = new OppositeToggleButtonsCommand(writingModeToggle, () => panel.Mode = ShellModes.Transform);

            VisualClickable addTextBlock = interpreter.CreateFromProperties(PUIHelper.VisualClickableInstantiator, "Add_Text_Block");
            addTextBlock.ClickCommand = new ActionCommander<Clickable>(AddTextBlockCommander);

            Buttons.AddRange(writingModeToggle, transformModeToggle, addTextBlock);

            ActivateWritingMode();
            transformModeToggle.SetCheckStatus(false); // don't actually need this line; its here in case of broken UI.
        }

        public void ActivateWritingMode()
        {
            (Buttons[0] as ToggleButton).Toggle();
        }

        public void ActivateTransformMode()
        {
            (Buttons[1] as ToggleButton).Toggle();
        }

        private void AddTextBlockCommander(Clickable clickable)
        {
            if (panel.DisplayedTab is not PageContentTab pageContentTab)
            {
                return;
            }

            PageTextContent content = ContentGetter.GetTextContent();
            content.Location = new Point(100, 100);
            content.Size = new Point(200, 50);

            pageContentTab.AddContent(content);
            ActivateWritingMode();

            pageContentTab.SimulateMouseButtonStateInput(new PopWindowMouseEventArgs(MouseButtons.Left,
                                                                                     MouseButtons.Left,
                                                                                     MouseButtons.None,
                                                                                     pageContentTab.GetContentScreenLocation(content)));
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
                    thisButton.Toggle(); // toggle back
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
