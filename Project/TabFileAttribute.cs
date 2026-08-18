using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Project
{
    [AttributeUsage(AttributeTargets.Class, AllowMultiple = false)]
    internal class TabFileAttribute : Attribute
    {
        public string FileTypeName { get; }
        public string Extension { get; }

        /// <summary>Initializes a new instance of TabFileAttribute class.</summary>
        /// <param name="fileTypeName">Displayable file type name of the tab.</param>
        /// <param name="extension">File extension of a file of the tab.</param>
        public TabFileAttribute(string fileTypeName, string extension)
        {
            FileTypeName = fileTypeName;
            Extension = extension;
        }
    }
}
