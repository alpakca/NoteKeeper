using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal interface IPropertyCollection
    {
        public IEnumerable<IPropertyEntry> Properties { get; }
    }
}
