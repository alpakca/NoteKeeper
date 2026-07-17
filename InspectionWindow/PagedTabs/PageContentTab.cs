using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal abstract class PageContentTab : Tab
    {
        public abstract void AddContent(IPageContent content);
        public abstract Point GetContentScreenLocation(IPageContent content);
    }
}
