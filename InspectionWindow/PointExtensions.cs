using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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

        public static Point Abs(this Point point)
        {
            return new Point(Math.Abs(point.X), Math.Abs(point.Y));
        }
    }

    internal static class PageContentUtil
    {
        public static (Point TransformedLocation, SpriteEffects Effects) GetTransformedLocation(Point location, Point size)
        {
            SpriteEffects effects = SpriteEffects.None;

            if (size.X < 0)
            {
                location.X += size.X;
                effects |= SpriteEffects.FlipHorizontally;
            }

            if (size.Y < 0)
            {
                location.Y += size.Y;
                effects |= SpriteEffects.FlipVertically;
            }

            return (location, effects);
        }

        public static Rectangle Normalized(this Rectangle rect)
        {
            if (rect.Width < 0)
            {
                rect.X += rect.Width;
                rect.Width = -rect.Width;
            }

            if (rect.Height < 0)
            {
                rect.Y += rect.Height;
                rect.Height = -rect.Height;
            }

            return rect;
        }
    }
}
