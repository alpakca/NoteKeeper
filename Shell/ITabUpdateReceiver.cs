using NoteKeeper.InspectionWindow;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell
{
    internal interface ITabUpdateReceiver
    {
        public void DisplayedTabChanged(Tab setTab);
        public void ZoomChanged();
    }
}
