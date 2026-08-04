namespace NoteKeeper
{
    public class Program
    {
        public static void Main(string[] args)
        {
            if (!SingleInstanceChecker.TryStart(args))
            {
                return;
            }

            using var game = new Game1(args);
            game.Run();

            SingleInstanceChecker.ReleaseMutex();
        }
    }
}