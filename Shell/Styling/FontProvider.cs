using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Text;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

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

        public static event Action FontChanged;

        public static void DrawText(int size, bool isBold, string text, Vector2 position, Color color)
        {
            DrawHelper.Batch.DrawString(isBold ? _loadedBoldFontSizes[size] : _loadedFontSizes[size],
                                        text, position, color);
        }

        public static float MeasureWidth(int size, bool isBold, string text)
        {
            return (isBold ? _loadedBoldFontSizes[size] : _loadedFontSizes[size]).MeasureString(text).X;
        }

        public static int GetLineSpacing(int size, bool isBold)
        {
            return isBold ? _loadedBoldFontSizes[size].LineSpacing : _loadedFontSizes[size].LineSpacing;
        }

        public static void UpdateProcesserFont(TextProcesser processer, int size, bool isBold)
        {
            processer.Font = isBold ? _loadedBoldFontSizes[size] : _loadedFontSizes[size];
        }
    }
}
