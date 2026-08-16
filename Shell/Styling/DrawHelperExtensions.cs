using Microsoft.Xna.Framework;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.Styling
{
    internal static class DrawHelperExtensions
    {
        extension(DrawHelper)
        {
            public static void DrawBorderedBackground(Rectangle bounds, Color borderColor, Color fillColor, float borderThickness)
            {
                DrawHelper.DrawBackground(bounds, borderColor);
                DrawHelper.DrawBackground(new Rectangle(bounds.X + (int)borderThickness, bounds.Y + (int)borderThickness,
                                                        bounds.Width - (int)(borderThickness * 2), bounds.Height - (int)(borderThickness * 2)),
                                          fillColor);
            }
        }
    }
}
