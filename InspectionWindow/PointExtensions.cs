using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow
{
    internal static class PointExtensions
    {
        public static Point Times(this Point point, float multiplier)
        {
            return new Point((int)(point.X * multiplier), (int)(point.Y * multiplier));
        }
    }
}
