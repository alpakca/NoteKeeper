using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow;
using NoteKeeper.InspectionWindow.SinglePage;
using PopUpUI.Elements;
using PopUpUI.Elements.Visual;
using PopUpUI.Input;
using System;
using System.Collections.Generic;
using System.Text;

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

                Boundaries = new Rectangle(100, 60, 20, 20),
                UncheckedAnimation = new PopUpUI.Surface.Animation2D([Util.GetTexture(20, 20, Color.Green)]),
                CheckedAnimation = new PopUpUI.Surface.Animation2D([Util.GetTexture(20, 20, Color.Gray)]),

                SelectablePacket = selectablePacket,
                HoldPacket = holdPacket,

                ToggleCommand = new ActionCommander<Clickable>(_ => panel.Mode = ShellModes.Writing)
            });

            Buttons.Add(new VisualToggleButton
            {
                Name = "Transform Mode Toggle",

                Boundaries = new Rectangle(130, 60, 20, 20),
                UncheckedAnimation = new PopUpUI.Surface.Animation2D([Util.GetTexture(20, 20, Color.Red)]),
                CheckedAnimation = new PopUpUI.Surface.Animation2D([Util.GetTexture(20, 20, Color.Blue)]),

                SelectablePacket = selectablePacket,
                HoldPacket = holdPacket,

                ToggleCommand = new ActionCommander<Clickable>(_ => panel.Mode = ShellModes.Transform)
            });

            Buttons.Add(new VisualClickable
            {
                Name = "Text Content Add Button",

                Boundaries = new Rectangle(160, 60, 20, 20),
                DefaultAnimation = new PopUpUI.Surface.Animation2D([Util.GetTexture(20, 20, Color.Beige)]),

                SelectablePacket = selectablePacket,
                HoldPacket = holdPacket,

                ClickCommand = new ActionCommander<Clickable>(_ =>
                {
                    PageTextContent content = ContentGetter.GetTextContent();
                    content.Location = new Point(100, 100);
                    content.Size = new Point(200, 50);

                    panel.DisplayedTab.AddContent(content);
                    panel.Mode = ShellModes.Writing;

                    panel.DisplayedTab.SimulateMouseButtonStateInput(new PopWindowMouseEventArgs(MouseButtons.Left,
                                                                                                 MouseButtons.Left,
                                                                                                 MouseButtons.None,
                                                                                                 panel.DisplayedTab.GetContentScreenLocation(content)));
                })
            });
        }
    }
}
