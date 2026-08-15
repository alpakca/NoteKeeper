using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.Theming
{
    internal static class ThemeService
    {
        private const string AssetsFolderRelativePath = "assets/";
        private static readonly Dictionary<string, Theme> _loadedThemes;

        public static ResolvedTheme CurrentTheme { get; private set; }
        public static event Action ThemeChanged;

        public static void RegisterTheme(Theme theme)
        {
            _loadedThemes.Add(theme.Id, theme);
        }

        public static bool SetTheme(string themeId)
        {
            if (!_loadedThemes.TryGetValue(themeId, out Theme theme))
            {
                return false;
            }

            HashSet<string> visitedBuffer = new HashSet<string>();

            Dictionary<string, Color> colors = new Dictionary<string, Color>();
            foreach (var colorId in ColorRegistry.RegisteredIds)
            {
                colors[colorId] = ResolveColor(theme, colorId, visitedBuffer);
                visitedBuffer.Clear();
            }

            Dictionary<string, Texture2D> icons = new Dictionary<string, Texture2D>();
            foreach (var iconId in IconRegistry.RegisteredIds)
            {
                icons[iconId] = ResolveIcon(theme, iconId, visitedBuffer);
                visitedBuffer.Clear();
            }

            CurrentTheme = new ResolvedTheme(theme.Id, colors, icons);
            ThemeChanged?.Invoke();

            return true;
        }

        private static Color ResolveColor(Theme theme, string id, HashSet<string> visitedIds)
        {
            if (!visitedIds.Add(id))
            {
                throw new InvalidOperationException("Circular fallback found on id: " + id);
            }

            ColorDefinition definition = ColorRegistry.Get(id)
                ?? throw new InvalidOperationException("No definition found at the base of fallbacks: " + id);

            if (theme.Colors.TryGetValue(definition.Id, out Color color))
            {
                return color;
            }

            return ResolveColor(theme, definition.FallbackColorId, visitedIds);
        }

        private static Texture2D ResolveIcon(Theme theme, string id, HashSet<string> visitedIds)
        {
            if (!visitedIds.Add(id))
            {
                throw new InvalidOperationException("Circular fallback found on id: " + id);
            }

            IconDefinition definition = IconRegistry.Get(id)
                ?? throw new InvalidOperationException("No definition found at the base of fallbacks: " + id);

            if (theme.Icons.TryGetValue(definition.Id, out string path))
            {
                return ContentAssistant.FromLoaded<Texture2D>(AssetsFolderRelativePath + path);
            }

            return ResolveIcon(theme, definition.FallbackIconId, visitedIds);
        }
    }
}
