using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class SubContextMenuButton : IContextMenuItem, IHasRightIcon
    {
        private Point location;
        private Point size;

        public Point Location
        {
            get => location;
            set
            {
                this.location = value;

                if (this.location.X + size.X + SubMenu.Boundaries.Width < WindowOptions.Size.X)
                {
                    SubMenu.SetLocation(new Point(this.location.X + size.X, value.Y));
                }
                else
                {
                    SubMenu.SetLocation(new Point(this.location.X - SubMenu.Boundaries.Width, value.Y));
                }
            }
        }

        public Point Size => size;
        public ContextMenu SubMenu { get; }

        public Texture2D? RightIcon { get; init; }

        public SubContextMenuButton(ContextMenu subMenu)
        {
            SubMenu = subMenu;
        }

        public void SetActualSize(Point size)
        {
            this.size = size;
        }

        public void Selected()
        {
        }
    }
}
