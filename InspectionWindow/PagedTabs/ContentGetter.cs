using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Text;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs
{
    internal static class ContentGetter
    {
        public static PageTextContent GetTextContent(string fontName)
        {
            return new PageTextContent(fontName);
        }

        public static PageImageContent GetImageContent(string imagePath)
        {
            return new PageImageContent(DrawHelper.FromFile(imagePath));
        }
    }
}
