using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal class NotePage
    {
        private readonly List<IPageContent> contents;

        public int Width { get; set; }
        public int Height { get; set; }
        public List<IPageContent> Contents => contents;

        public NotePage()
        {
            contents = new List<IPageContent>();
        }
    }
}
