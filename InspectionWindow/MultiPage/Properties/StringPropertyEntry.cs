using System;
using System.Collections.Generic;

namespace NoteKeeper.InspectionWindow.MultiPage.Properties
{
    internal class StringPropertyEntry : IUnderlyingValueSetter
    {
        private string _value;
        private readonly Action<string> _valueSetAction;

        public string Name { get; }
        public string CurrentValueAsString => _value;
        public bool HasDropdownOptions => false;

        public StringPropertyEntry(string name, Action<string> valueSetAction)
        {
            _valueSetAction = valueSetAction;
        }

        public void SetUnderlyingValue(object value)
        {
            _value = (string)value;
        }

        public void SetValue(string input)
        {
            _value = input;
            _valueSetAction(input);
        }

        public bool TryValidate(string input)
        {
            return true;
        }

        public bool IsCurrentValueEqual(string input)
        {
            return input == _value;
        }

        public IEnumerable<string> GetDropdownOptions()
        {
            throw new NotSupportedException();
        }

        public void SetDropdownOptionValue(string option)
        {
            throw new NotSupportedException();
        }

        public bool IsCurrentDropdownOptionEqual(string option)
        {
            throw new NotSupportedException();
        }
    }
}
