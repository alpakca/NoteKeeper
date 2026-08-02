using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal interface ITabSerializer
    {
        public string FileExtension { get; }

        public void Load();
        public void Save();
    }
}
