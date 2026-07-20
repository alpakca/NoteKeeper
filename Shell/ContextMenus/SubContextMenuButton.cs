using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class SubContextMenuButton : IContextMenuItem
    {
        private readonly SubMenuButtonRenderer _renderer;

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
        public IContextMenuItemRenderer Renderer => _renderer;
        public ContextMenu SubMenu { get; }

        public SubContextMenuButton(ContextMenu subMenu, Texture2D icon, string text, SpriteFont font, Texture2D arrowTexture,
                                    Texture2D background, Texture2D hoverOverlay, Texture2D heldOverlay)
        {
            _renderer = new SubMenuButtonRenderer(icon, text, font, arrowTexture, background, hoverOverlay, heldOverlay);
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
