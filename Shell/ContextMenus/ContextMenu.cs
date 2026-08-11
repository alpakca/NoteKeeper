using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenu
    {
        private const int FrameMargin = 2;

        private readonly IContextMenuEntry[] _entries;

        public Point Location { get; set; }
        /// <summary>Size of context menu without frame margin. Its where IContextMenuItem/s will belong in.</summary>
        public Point Size { get; set; }

        public Point FramedLocation => Location - new Point(FrameMargin);
        /// <summary>Size of context menu with frame margin. Its the total rendered size.</summary>
        public Point FramedSize => Size + new Point(FrameMargin * 2);

        public IContextMenuEntry[] Entries => _entries;

        public ContextMenu(IContextMenuEntry[] entries)
        {
            _entries = entries;
        }

        public bool InsideBoundaries(Point p)
        {
            return FramedLocation.X <= p.X && p.X < FramedLocation.X + FramedSize.X &&
                   FramedLocation.Y <= p.Y && p.Y < FramedLocation.Y + FramedSize.Y;
        }
    }
}
