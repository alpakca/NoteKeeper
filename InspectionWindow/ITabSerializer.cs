using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal interface ITabSerializer
    {
        public string FileExtension { get; }
        /// <summary>Indicates if tab modified after its last save.</summary>
        public bool IsModified { get; }

        public void Load();
        public void Save();
    }
}
