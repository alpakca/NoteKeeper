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

        public Texture2D TabCloseButtonDefaultBackground { get; init; }
        public Texture2D TabCloseButtonHoverBackground { get; init; }
        public Texture2D TabCloseButtonHeldBackground { get; init; }
    }
}
