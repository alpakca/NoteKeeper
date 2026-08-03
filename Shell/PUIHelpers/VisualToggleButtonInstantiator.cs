using Microsoft.Xna.Framework;
using PopUpUI.Elements.Visual;
using PopUpUI.Input;
using PopUpUI.PUI;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal class VisualToggleButtonInstantiator : IDescriptorInstantiator<VisualToggleButton>
    {
        public string TypeName => "VisualToggleButton";

        public VisualToggleButton Instantiate(PUIDescriptor desc)
        {
            VisualToggleButton button = new VisualToggleButton
            {
                Name = desc.GetPropertyOrDefault("Name", ""),
                Boundaries = desc.GetPropertyOrDefault("Boundaries", Rectangle.Empty),
                CheckedAnimation = desc.GetPropertyOrDefault<Animation2D>("CheckedAnimation", null),
                UncheckedAnimation = desc.GetPropertyOrDefault<Animation2D>("UncheckedAnimation", null),
                UnenabledAnimation = desc.GetPropertyOrDefault<Animation2D>("UnenabledAnimation", null),

                SelectablePacket = desc.GetPropertyOrDefault<SelectablePacket>("SelectablePacket", null),
                HoldPacket = desc.GetPropertyOrDefault<HoldablePacket>("HoldablePacket", null)
            };

            return button;
        }
    }
}
