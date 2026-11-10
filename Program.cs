using System;
using System.IO;

namespace NoteKeeper
{
    public class Program
    {
        private const string RootPathFileRelativePath = "RootPath";
        private const string PreferencesFileRelativePath = "Preferences";

        public static string RootPath { get; private set; }
        public static string PreferencesPath { get; private set; }

        public static void Main(string[] args)
        {
            using var game = new Game1(args);

            if (!SingleInstanceChecker.TryStart(args))
            {
                return;
            }

            RootPath = File.ReadAllText(AppContext.BaseDirectory + RootPathFileRelativePath);
            PreferencesPath = AppContext.BaseDirectory + PreferencesFileRelativePath;

            game.Run();
        }
    }
}