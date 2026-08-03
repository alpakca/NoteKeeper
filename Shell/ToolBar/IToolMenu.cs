using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ToolBar
{
    internal interface IToolMenu
    {
        public ToolButton[] Buttons { get; }
    }
}
