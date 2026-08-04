using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Project
{
    [AttributeUsage(AttributeTargets.Class)]
    internal class TabFileExtensionAttribute : Attribute
    {
        public string Extension { get; }

        public TabFileExtensionAttribute(string extension)
        {
            Extension = extension;
        }
    }
}
