using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.PUIHelpers
{
    internal class TupleParseException : Exception
    {
        public TupleParseException(string m) : base(m)
        {

        }
    }

    internal class VariableUnknownPropertyException : Exception
    {
        public VariableUnknownPropertyException(string propertyName) : base($"Unknown property name encountered: {propertyName}")
        {

        }
    }
}
