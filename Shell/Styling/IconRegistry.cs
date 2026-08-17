using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace NoteKeeper.Shell.Styling
{
    internal static class IconRegistry
    {
        private static readonly Dictionary<string, IconDefinition> _definitions = new Dictionary<string, IconDefinition>();

        public const string CloseButtonId = "base.closebutton";

        public static IEnumerable<string> RegisteredIds => _definitions.Keys;

        [ModuleInitializer]
        public static void InitializeBaseIcons()
        {
            Register(new IconDefinition 
            {
                Id = CloseButtonId, 
                DefaultPath = @"shell\CloseButton.png"
            });
        }

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
