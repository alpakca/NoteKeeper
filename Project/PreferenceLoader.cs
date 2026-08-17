using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.IO;
using System.Text.Json;

namespace NoteKeeper.Project
{
    internal static class PreferenceLoader
    {
        public static AppPreferences Load()
        {
            AppPreferences preferences;
            try
            {
                string json = File.ReadAllText(Program.PreferencesPath);
                preferences = JsonSerializer.Deserialize<AppPreferences>(json);
            }
            catch (Exception e) when (e is JsonException || e is IOException)
            {
                preferences = AppPreferences.Default;
            }

            ValidatePreference(preferences);

            WindowOptions.Position = new Point(preferences.WindowLeft, preferences.WindowTop);
            WindowOptions.Size = new Point(preferences.WindowWidth, preferences.WindowHeight);

            return preferences;
        }

        public static void Save(Viewport viewport)
        {
            AppPreferences preferences = new AppPreferences
            {
                WindowLeft = WindowOptions.Position.X,
                WindowWidth = viewport.Width,
                WindowTop = WindowOptions.Position.Y,
                WindowHeight = viewport.Height
            };

            ValidatePreference(preferences);

            File.WriteAllText(Program.PreferencesPath, JsonSerializer.Serialize(preferences));
        }

        private static void ValidatePreference(AppPreferences preferences)
        {
            if (preferences.WindowWidth == 0)
                preferences.WindowWidth = AppPreferences.Default.WindowWidth;

            if (preferences.WindowHeight == 0)
                preferences.WindowHeight = AppPreferences.Default.WindowHeight;
        }
    }
}
