using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.Theming
{
    internal class ColorDefinition
    {
        public string Id { get; init; }
        public Color DefaultValue { get; init; }
        public string? FallbackColorId { get; init; }
    }
}
