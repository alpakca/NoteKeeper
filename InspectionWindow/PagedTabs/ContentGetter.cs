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
        public static PageTextContent GetTextContent()
        {
            return new PageTextContent(new TextProcesser { Font = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans") });
        }

        public static PageImageContent GetImageContent(string imagePath)
        {
            return new PageImageContent(DrawHelper.FromFile(imagePath));
        }
    }
}
