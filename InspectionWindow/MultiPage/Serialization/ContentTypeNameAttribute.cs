using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.Serialization
{
    internal class ContentTypeNameAttribute : Attribute
    {
        public string TypeName { get; }

        public ContentTypeNameAttribute(string typeName)
        {
            TypeName = typeName;
        }
    }
}
