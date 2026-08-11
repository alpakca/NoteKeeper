using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class SubContextMenuButton : IContextMenuItem
    {
        public Point Location { get; set; }
        public Point Size { get; set; }

        public ContextMenu SubMenu { get; }

        public Texture2D RightIconTexture { get; }
        public string LeftText { get; }

        public SubContextMenuButton(ContextMenu subMenu, Texture2D rightIconTexture, string leftText)
        {
            SubMenu = subMenu;

            RightIconTexture = rightIconTexture;
            LeftText = leftText;
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
