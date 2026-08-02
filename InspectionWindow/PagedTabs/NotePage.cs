using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal class NotePage
    {
        public ContentCollection Contents { get; }

        public NotePage()
        {
            Contents = new ContentCollection();
        }
    }
}
