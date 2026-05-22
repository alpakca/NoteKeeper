using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal interface IPageContent
    {
        public Point Size { get; set; }
        public Point Location { get; set; }
        public void Render(Point pageLocation);
    }
}
