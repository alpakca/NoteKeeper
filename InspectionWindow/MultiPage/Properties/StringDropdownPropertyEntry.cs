using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.Properties
{
    internal class StringDropdownPropertyEntry : IUnderlyingValueSetter
    {
        private string _value;
        private readonly IEnumerable<string> _options;
        private readonly Action<string> _valueSetAction;

        public string Name { get; }
        public string CurrentValueAsString => _value;
        public bool HasDropdownOptions => true;

        public StringDropdownPropertyEntry(string name, IEnumerable<string> options, Action<string> valueSetAction)
        {
            Name = name;
            _options = options;
            _valueSetAction = valueSetAction;
        }

        public void SetUnderlyingValue(object value)
        {
            _value = (string)value;
        }

        public void SetValue(string input)
        {
            _value = input;
            _valueSetAction(_value);
        }

        public IEnumerable<string> GetDropdownOptions()
        {
            return _options;
        }

        public bool TryValidate(string input)
        {
            return _options.Contains(input);
        }

        public void SetDropdownOptionValue(string option)
        {
            SetValue(option);
        }
    }
}
