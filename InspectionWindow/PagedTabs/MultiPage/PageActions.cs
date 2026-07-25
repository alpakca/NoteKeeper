using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class PageActions
    {
        private readonly TabHistory _history;

        public PageActions(TabHistory history)
        {
            _history = history;
        }

        public void SetContentLocation(IPageContent content, Point newLocation, int newPageIndex)
        {

        }

        public void SetContentSize(IPageContent content, Point newSize)
        {

        }

        public void AddContent(IPageContent content)
        {

        }

        public void DeleteContent(IPageContent content)
        {

        }
    }
}
