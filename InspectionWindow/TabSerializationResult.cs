using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal enum TabSerializationResult
    {
        Success,
        FileNotFound,
        IOError,
        WrongFileFormat
    }
}
