using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.History
{
    internal interface IHistoryAction
    {
        public string UIDescription { get; }

        public void Run();
        public void Undo();
    }
}
