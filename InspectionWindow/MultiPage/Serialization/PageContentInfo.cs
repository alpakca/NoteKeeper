using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Net.Mime;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.Serialization
{
    internal class PageContentInfo
    {
        public Point Location { get; init; }
        public Point Size { get; init; }
    }

    [ContentTypeName("ImageContent")]
    internal class ImageContentInfo : PageContentInfo
    {
        public string ImagePath { get; init; }
    }

    [ContentTypeName("TextContent")]
    internal class TextContentInfo : PageContentInfo
    {
        public string Text { get; init; }
        public string FontName { get; init; }
        public int FontSize { get; init; }
    }
}
