using Microsoft.Xna.Framework;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenu
    {
        public Point Location { get; set; }
        public Point Size { get; set; }

        public IContextMenuEntry[] Entries { get; }

        public ContextMenu(IContextMenuEntry[] entries)
        {
            Entries = entries;
        }
    }
}
