using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class GroupLineItem : IContextMenuEntry
    {
        public Point Location { get; set; }
        public Point Size { get; set; }
    }
}
