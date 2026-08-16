using System.Collections.Generic;

namespace NoteKeeper.Shell.Styling
{
    internal static class IconRegistry
    {
        private static readonly Dictionary<string, IconDefinition> _definitions = new Dictionary<string, IconDefinition>();

        public static IEnumerable<string> RegisteredIds => _definitions.Keys;

        public static void Register(IconDefinition definition)
        {
            _definitions[definition.Id] = definition;
        }

        public static IconDefinition Get(string id)
        {
            return _definitions.TryGetValue(id, out IconDefinition def) ? def : null;
        }
    }
}
