using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell.Theming
{
    internal static class ColorRegistry
    {
        public const string BackgroundId = "base.background";
        public const string BorderId = "base.border";
        /// <summary>Color id of color to be used in surfaces above background.</summary>
        public const string SurfaceId = "base.surface";
        public const string HoverOverlay = "base.hoveroverlay";
        public const string HeldOverlay = "base.heldoverlay";
        
        private static readonly Dictionary<string, ColorDefinition> _definitions = new Dictionary<string, ColorDefinition>();

        public static IEnumerable<string> RegisteredIds => _definitions.Keys;

        public static void Register(ColorDefinition definition)
        {
            _definitions[definition.Id] = definition;
        }

        public static ColorDefinition? Get(string id)
        {
            return _definitions.TryGetValue(id, out ColorDefinition? def) ? def : null;
        }
    }
}
