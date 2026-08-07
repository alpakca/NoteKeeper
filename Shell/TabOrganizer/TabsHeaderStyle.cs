using Microsoft.Xna.Framework.Graphics;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.TabOrganizer
{
    internal class TabsHeaderStyle
    {
        public Texture2D HeaderBackground { get; init; }

        public Texture2D DisplayedTabBackground { get; init; }
        public Texture2D TabDefaultBackground { get; init; }
        public Texture2D TabHoveredBackground { get; init; }

        public SpriteFont DefaultFont { get; init; }
        public SpriteFont DisplayedTabFont { get; init; }

        public Texture2D TabCloseButtonIcon { get; init; }
        public Texture2D TabListButtonIcon { get; init; }

        public Texture2D ButtonHoverOverlay { get; init; }
        public Texture2D ButtonHeldOverlay { get; init; }
    }
}
