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
    internal class VisualClickableInstantiator : IDescriptorInstantiator<VisualClickable>
    {
        public string TypeName => "VisualClickable";

        public VisualClickable Instantiate(PUIDescriptor desc)
        {
            VisualClickable clickable = new VisualClickable
            {
                Name = desc.GetPropertyOrDefault("Name", ""),
                Boundaries = desc.GetPropertyOrDefault("Boundaries", Rectangle.Empty),
                DefaultAnimation = desc.GetPropertyOrDefault<Animation2D>("DefaultAnimation", null),
                HoldAnimation = desc.GetPropertyOrDefault<Animation2D>("HoldAnimation", null),
                HoverAnimation = desc.GetPropertyOrDefault<Animation2D>("HoverAnimation", null),
                UnenabledAnimation = desc.GetPropertyOrDefault<Animation2D>("UnenabledAnimation", null),

                SelectablePacket = desc.GetPropertyOrDefault<SelectablePacket>("SelectablePacket", null),
                HoldPacket = desc.GetPropertyOrDefault<HoldablePacket>("HoldablePacket", null)
            };

            return clickable;
        }
    }
}
