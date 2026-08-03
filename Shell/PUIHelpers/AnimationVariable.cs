using Microsoft.Xna.Framework.Graphics;
using PopUpUI.PUI;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal class AnimationVariableCreator : IPUIVariableCreator
    {
        public string TypeName => "Animation";

        public PUIVariable Get()
        {
            return new AnimationVariable();
        }
    }

    internal class AnimationVariable : PUIVariable
    {
        private Animation2D animation;

        public override object Value
        {
            get => animation;
            set => animation = Animation2D.FromTexture(ContentAssistant.FromLoaded<Texture2D>(value as string));
        }
    }
}
