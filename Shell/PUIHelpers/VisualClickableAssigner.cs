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
    internal class VisualClickableAssigner : IDescriptorAssigner<VisualClickable>
    {
        public string TypeName => "VisualClickable";

        public void Assign(PUIDescriptor desc, VisualClickable element)
        {
            element.Name = desc.GetPropertyOrDefault("Name", "");
            element.Boundaries = desc.GetPropertyOrDefault("Boundaries", Rectangle.Empty);
            element.DefaultAnimation = desc.GetPropertyOrDefault<Animation2D>("DefaultAnimation", null);
            element.HoldAnimation = desc.GetPropertyOrDefault<Animation2D>("HoldAnimation", null);
            element.HoverAnimation = desc.GetPropertyOrDefault<Animation2D>("HoverAnimation", null);
            element.UnenabledAnimation = desc.GetPropertyOrDefault<Animation2D>("UnenabledAnimation", null);

            element.SelectablePacket = desc.GetPropertyOrDefault<SelectablePacket>("SelectablePacket", null);
            element.HoldPacket = desc.GetPropertyOrDefault<HoldablePacket>("HoldablePacket", null);
        }
    }
}
