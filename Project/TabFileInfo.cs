using NoteKeeper.InspectionWindow;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Project
{
    internal class TabFileInfo
    {
        public string FileTypeName { get; init; }
        public string Extension { get; init; }
        public Func<string, Tab> Creator { get; init; }
    }
}
