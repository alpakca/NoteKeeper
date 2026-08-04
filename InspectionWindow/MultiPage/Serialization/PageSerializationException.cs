using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.Serialization
{
    internal class PageSerializationException : Exception
    {
        public PageSerializationException(string message) : base(message)
        {
        }
    }
}
