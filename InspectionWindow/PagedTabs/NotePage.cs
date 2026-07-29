using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal class NotePage
    {
        public List<IPageContent> Contents { get; }

        public NotePage()
        {
            Contents = new List<IPageContent>();
        }
    }
}
