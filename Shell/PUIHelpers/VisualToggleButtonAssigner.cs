using Microsoft.Xna.Framework;
using PopUpUI.Elements.Visual;
using PopUpUI.Input;
using PopUpUI.PUI;
using PopUpUI.Surface;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal class VisualToggleButtonAssigner : IDescriptorAssigner<VisualToggleButton>
    {
        public string TypeName => "VisualToggleButton";

        public void Assign(PUIDescriptor desc, VisualToggleButton button)
        {
            button.Name = desc.GetPropertyOrDefault("Name", "");
            button.Boundaries = desc.GetPropertyOrDefault("Boundaries", Rectangle.Empty);
            button.CheckedAnimation = desc.GetPropertyOrDefault<Animation2D>("CheckedAnimation", null);
            button.UncheckedAnimation = desc.GetPropertyOrDefault<Animation2D>("UncheckedAnimation", null);
            button.UnenabledAnimation = desc.GetPropertyOrDefault<Animation2D>("UnenabledAnimation", null);

            button.SelectablePacket = desc.GetPropertyOrDefault<SelectablePacket>("SelectablePacket", null);
            button.HoldPacket = desc.GetPropertyOrDefault<HoldablePacket>("HoldablePacket", null);
        }
    }
}