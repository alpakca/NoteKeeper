using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.Styling
{
    internal static class FontProvider
    {
        private static readonly Dictionary<int, SpriteFont> _loadedFontSizes = new Dictionary<int, SpriteFont>
        {
            { 12, ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_12") }, 
            { 16, ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_16") }, 
            { 24, ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_24") },
        };

        private static readonly Dictionary<int, SpriteFont> _loadedBoldFontSizes = new Dictionary<int, SpriteFont>
        {
            { 12, ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_Bold_12") },
            { 16, ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_Bold_16") },
            { 24, ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans_Bold_24") },
        };

        public static void DrawText(int size, string text, Vector2 position, Color color)
        {
            DrawHelper.Batch.DrawString(_loadedFontSizes[size], text, position, color);
        }

        public static void DrawTextBold(int size, string text, Vector2 position, Color color)
        {
            DrawHelper.Batch.DrawString(_loadedBoldFontSizes[size], text, position, color);
        }
    }
}
