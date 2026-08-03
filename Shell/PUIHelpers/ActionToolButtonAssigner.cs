using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.Shell.ToolBar;
using PopUpUI.PUI;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal class ActionToolButtonAssigner : IDescriptorAssigner<ActionToolButton>
    {
        public string TypeName => "ActionToolButton";

        public void Assign(PUIDescriptor desc, ActionToolButton element)
        {
            element.DefaultTexture = desc.GetPropertyOrDefault<Texture2D>("DefaultTexture", null);
            element.HoveredTexture = desc.GetPropertyOrDefault<Texture2D>("HoveredTexture", null);
            element.HeldTexture = desc.GetPropertyOrDefault<Texture2D>("HeldTexture", null);
            element.UnenabledTexture = desc.GetPropertyOrDefault<Texture2D>("UnenabledTexture", null);
        }
    }
}
