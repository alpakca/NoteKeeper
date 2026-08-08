using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.Shell.ToolBar;
using PopUpUI.PUI;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal class ToolButtonAssigner : IDescriptorAssigner<ActionToolButton>, IDescriptorAssigner<ToggleToolButton>
    {
        string IDescriptorAssigner<ActionToolButton>.TypeName => "ActionToolButton";
        string IDescriptorAssigner<ToggleToolButton>.TypeName => "ToggleToolButton";

        public void Assign(PUIDescriptor desc, ActionToolButton element)
        {
            element.DefaultTexture = desc.GetPropertyOrDefault<Texture2D>("DefaultTexture", null);
            element.HoveredTexture = desc.GetPropertyOrDefault<Texture2D>("HoveredTexture", null);
            element.HeldTexture = desc.GetPropertyOrDefault<Texture2D>("HeldTexture", null);
            element.UnenabledTexture = desc.GetPropertyOrDefault<Texture2D>("UnenabledTexture", null);
        }

        public void Assign(PUIDescriptor desc, ToggleToolButton element)
        {
            element.CheckedTexture = desc.GetPropertyOrDefault<Texture2D>("CheckedTexture", null);
            element.UncheckedTexture = desc.GetPropertyOrDefault<Texture2D>("UncheckedTexture", null);
            element.UnenabledTexture = desc.GetPropertyOrDefault<Texture2D>("UnenabledTexture", null);
        }
    }
}
