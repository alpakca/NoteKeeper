using System;
using System.IO;

namespace NoteKeeper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            AppDomain.CurrentDomain.UnhandledException += (sender, e) =>
            {
                File.WriteAllText(@"C:\Users\user\Desktop\C#\NoteKeeper\CrashLog.txt", ((Exception)e.ExceptionObject).StackTrace
                    + "\n\n" + ((Exception)e.ExceptionObject).Message
                    + "\n\n" + string.Join('\n', args)
                    + "\n\n" + SingleInstanceChecker.IsServer);
            };

            using var game = new Game1(args);

            if (!SingleInstanceChecker.TryStart(args))
            {
                return;
            }

            game.Run();
        }
    }
}