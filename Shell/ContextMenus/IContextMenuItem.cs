using Microsoft.Xna.Framework;
using PopUpUI.Elements;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal interface IContextMenuItem : IContextMenuEntry
    {
        public void Selected();
    }
}
