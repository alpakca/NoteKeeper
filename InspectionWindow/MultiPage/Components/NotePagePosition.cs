using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.Components
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

        public void Deconstruct(out int pageIndex, out Point pagePosition)
        {
            pageIndex = PageIndex;
            pagePosition = PagePosition;
        }
    }
}
