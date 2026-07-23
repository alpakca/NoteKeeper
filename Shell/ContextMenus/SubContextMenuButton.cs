using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class SubContextMenuButton : IContextMenuItem, IHasRightIcon
    {
        public Point Location { get; set; }
        public Point Size { get; set; }

        public ContextMenu SubMenu { get; }

        public Texture2D? RightIcon { get; init; }

        public SubContextMenuButton(ContextMenu subMenu)
        {
            SubMenu = subMenu;
        }

        public void Selected()
        {
        }

        public Point GetPossibleSubMenuPlacement()
        {
            if (Location.X + Size.X + SubMenu.FramedSize.X < WindowOptions.Size.X)
            {
                return new Point(Location.X + Size.X, Location.Y);
            }
            else
            {
                return new Point(Location.X - SubMenu.FramedSize.X, Location.Y);
            }
        }
    }
}
