using NoteKeeper.Shell.ToolBar;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class TabToolMenu : IToolMenu
    {
        public ToolButton[] Buttons { get; }

        public TabToolMenu()
        {
            Buttons = [];
        }
    }
}
