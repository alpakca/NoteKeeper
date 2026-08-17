using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Text;
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

        public static event Action FontChanged;

        public static void DrawText(int size, bool isBold, string text, Vector2 position, Color color)
        {
            DrawHelper.Batch.DrawString(isBold ? _loadedBoldFontSizes[size] : _loadedFontSizes[size],
                                        text, position, color);
        }

        public static void DrawCenteredText(int size, bool isBold, string text, Rectangle boundaries, Color color)
        {
            Vector2 textSize = MeasureSize(size, isBold, text);
            DrawText(size, isBold, text, new Vector2(boundaries.X + (boundaries.Width - textSize.X) / 2, boundaries.Y + (boundaries.Height - textSize.Y) / 2), color);
        }

        public static float MeasureWidth(int size, bool isBold, string text)
        {
            return (isBold ? _loadedBoldFontSizes[size] : _loadedFontSizes[size]).MeasureString(text).X;
        }

        public static float MeasureWidth(int size, bool isBold, StringBuilder builder)
        {
            return (isBold ? _loadedBoldFontSizes[size] : _loadedFontSizes[size]).MeasureString(builder).X;
        }

        public static Vector2 MeasureSize(int size, bool isBold, string text)
        {
            return (isBold ? _loadedBoldFontSizes[size] : _loadedFontSizes[size]).MeasureString(text);
        }

        public static int GetLineSpacing(int size, bool isBold)
        {
            return isBold ? _loadedBoldFontSizes[size].LineSpacing : _loadedFontSizes[size].LineSpacing;
        }

        public static void UpdateProcesserFont(TextProcesser processer, int size, bool isBold)
        {
            processer.Font = isBold ? _loadedBoldFontSizes[size] : _loadedFontSizes[size];
        }

        public static bool CanCharacterResolved(int size, bool isBold, char ch)
        {
            return (isBold ? _loadedBoldFontSizes[size] : _loadedFontSizes[size]).Characters.Contains(ch);
        }
    }
}
