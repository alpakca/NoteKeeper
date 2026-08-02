using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal abstract class PageContentTab : Tab
    {
        protected PageContentTab(string sourcePath) : base(sourcePath)
        {
        }

        public abstract void AddContent(IPageContent content);
        public abstract Point GetContentScreenLocation(IPageContent content);
    }
}
