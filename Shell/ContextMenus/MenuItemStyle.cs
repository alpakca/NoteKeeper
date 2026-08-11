using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class MenuItemStyle
    {
        public int Height { get; init; }
    }

    internal class MenuButtonStyle
    {
        public Color LeftTextColor { get; init; }
        public Point LeftIconSize { get; init; }

        public int IconHorizontalPadding { get; init; }
        public int LeftRightPartPadding { get; init; }
        public int HorizontalPadding { get; init; }
    }

    internal class ActionMenuItemStyle : MenuItemStyle
    {
        public MenuButtonStyle ButtonStyle { get; init; }
        public Color RightTextColor { get; init; }
    }

    internal class SubContextMenuItemStyle : MenuItemStyle
    {
        public MenuButtonStyle ButtonStyle { get; init; }
        public Point RightIconSize { get; init; }
    }

    internal class GroupLineStyle : MenuItemStyle
    {
        public Color LineColor { get; init; }
        public int Thickness { get; init; }
    }
}
