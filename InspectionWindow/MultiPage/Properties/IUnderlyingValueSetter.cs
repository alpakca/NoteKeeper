using NoteKeeper.Shell.PropertyExplorer;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.Properties
{
    internal interface IUnderlyingValueSetter : IPropertyEntry
    {
        public void SetUnderlyingValue(object value);
    }
}
