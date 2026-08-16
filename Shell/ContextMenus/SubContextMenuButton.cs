using Microsoft.Xna.Framework;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class SubContextMenuButton : IContextMenuItem
    {
        public Point Location { get; set; }
        public Point Size { get; set; }

        public ContextMenu SubMenu { get; }
        public string LeftText { get; }

        public SubContextMenuButton(ContextMenu subMenu, string leftText)
        {
            SubMenu = subMenu;
            LeftText = leftText;
        }

        public void Selected()
        {
        }
    }
}
