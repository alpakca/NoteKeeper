using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal readonly struct NotePagePosition
    {
        public int PageIndex { get; }
        public Point PagePosition { get; }

        public NotePagePosition(int pageIndex, Point pagePosition)
        {
            PageIndex = pageIndex;
            PagePosition = pagePosition;
        }
    }
}
