using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;

namespace NoteKeeper.Shell.Theming
{
    internal class Theme
    {
        public string Id { get; init; }
        public string DisplayName { get; init; }
        public IReadOnlyDictionary<string, Color> Colors { get; init; }
        public IReadOnlyDictionary<string, string> Icons { get; init; }
    }
}
