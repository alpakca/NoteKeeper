using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.Theming
{
    internal class IconDefinition
    {
        public string Id { get; init; }
        public string? DefaultPath { get; init; }
        public string? FallbackIconId { get; init; }
    }
}
