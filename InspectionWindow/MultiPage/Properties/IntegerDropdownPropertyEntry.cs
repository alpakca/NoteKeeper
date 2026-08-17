using NoteKeeper.Shell.PropertyExplorer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NoteKeeper.InspectionWindow.MultiPage.Properties
{
    internal class IntegerDropdownPropertyEntry : IUnderlyingValueSetter
    {
        private int _value;
        private readonly IEnumerable<string> _options;
        private readonly Action<int> _valueSetAction;

        public string Name { get; }
        public string CurrentValueAsString => _value.ToString();
        public bool HasDropdownOptions => true;

        public IntegerDropdownPropertyEntry(string name, int[] options, Action<int> valueSetAction)
        {
            Name = name;
            _options = options.Select(v => v.ToString());
            _valueSetAction = valueSetAction;
        }

        public void SetUnderlyingValue(object value)
        {
            _value = (int)value;
        }

        public void SetValue(string input)
        {
            _value = int.Parse(input);
            _valueSetAction(_value);
        }

        public bool TryValidate(string input)
        {
            return _options.Contains(input) && int.TryParse(input, out _);
        }

        public bool IsCurrentValueEqual(string input)
        {
            return input == _value.ToString();
        }

        public IEnumerable<string> GetDropdownOptions()
        {
            return _options;
        }

        public void SetDropdownOptionValue(string option)
        {
            SetValue(option);
        }

        public bool IsCurrentDropdownOptionEqual(string option)
        {
            return IsCurrentValueEqual(option);
        }
    }
}
