using System;
using System.IO;

namespace NoteKeeper
{
    public class Program
    {
        private const string RootPathFileRelativePath = "RootPath.txt";

        public static string RootPath { get; private set; }

        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                File.WriteAllText(@"C:\Users\user\Desktop\C#\NoteKeeper\CrashLog.txt", ((Exception)e.ExceptionObject).StackTrace
                    + "\n\n" + ((Exception)e.ExceptionObject).Message
                    + "\n\n" + string.Join('\n', args)
                    + "\n\n" + SingleInstanceChecker.IsServer);
            };

#if DEBUG
            args = [@"C:\Users\user\Desktop\XXTest.mlpage", @"C:\Users\user\Desktop\MultiPageTest.mlpage"];
#endif

#if DEBUG
            RootPath = File.ReadAllText(@"C:\Users\user\Desktop\C#\NoteKeeper\NoteKeeper\RootPath.txt");
#else
            RootPath = File.ReadAllText(RootPathFileRelativePath);
#endif

            using var game = new Game1(args);

            if (!SingleInstanceChecker.TryStart(args))
            {
                return;
            }

            game.Run();
        }
    }
}