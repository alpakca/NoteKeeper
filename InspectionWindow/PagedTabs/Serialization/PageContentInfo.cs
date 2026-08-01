using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.Serialization
{
    internal class PageContentInfo
    {
        public string TypeName { get; init; }
        public Point Location { get; init; }
        public Point Size { get; init; }
    }
}
