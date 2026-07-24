using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PropertyExplorer
{
    internal interface IPropertyEntry
    {
        public string Name { get; }
        public string CurrentValueAsString { get; }

        public bool TryValidate(string input);
        public void SetValue(string input);

        public bool HasDropdownOptions { get; }

        public IEnumerable<string> GetDropdownOptions();
        public void SetDropdownOptionValue(string option);
    }
}
