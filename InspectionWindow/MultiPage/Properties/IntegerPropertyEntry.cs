using System;
using System.Collections.Generic;

namespace NoteKeeper.InspectionWindow.MultiPage.Properties
{
    internal class IntegerPropertyEntry : IUnderlyingValueSetter
    {
        private int _value;
        private readonly Action<int> _valueSetAction;

        public string Name { get; }
        public string CurrentValueAsString => _value.ToString();
        public bool HasDropdownOptions => false;

        public IntegerPropertyEntry(string name, Action<int> valueSetAction)
        {
            Name = name;
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
            return int.TryParse(input, out _);
        }

        public IEnumerable<string> GetDropdownOptions()
        {
            throw new NotSupportedException();
        }

        public void SetDropdownOptionValue(string option)
        {
            throw new NotSupportedException();
        }
    }
}
