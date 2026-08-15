using Microsoft.Xna.Framework;
using System.Collections.Generic;

namespace NoteKeeper.Shell.Styling
{
    internal class Theme
    {
        public string Id { get; init; }
        public string DisplayName { get; init; }
        public IReadOnlyDictionary<string, Color> Colors { get; init; }
        public IReadOnlyDictionary<string, string> Icons { get; init; }
    }
}
