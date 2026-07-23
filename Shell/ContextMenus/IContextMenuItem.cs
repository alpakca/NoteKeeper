using Microsoft.Xna.Framework;
using PopUpUI.Elements;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal interface IContextMenuItem
    {
        public Point Location { get; set; }
        public Point Size { get; set; }
        public void Selected();
    }
}
