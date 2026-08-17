using System.Text.Json.Serialization;

namespace NoteKeeper.Project
{
    internal class AppPreferences
    {
        [JsonIgnore]
        public static AppPreferences Default { get; } = new AppPreferences
        {
            WindowLeft = 20,
            WindowWidth = 1000,
            WindowTop = 30,
            WindowHeight = 800
        };

        public int WindowLeft { get; set; }
        public int WindowWidth { get; set; }
        public int WindowTop { get; set; }
        public int WindowHeight { get; set; }
    }
}
