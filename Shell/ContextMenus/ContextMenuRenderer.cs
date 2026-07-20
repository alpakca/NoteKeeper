using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenuRenderer
    {
        private readonly IEnumerable<SubMenuEntry> openMenus;

        public ContextMenuRenderer(IEnumerable<SubMenuEntry> openMenus)
        {
            this.openMenus = openMenus;
        }

        public void Render(RenderTarget2D windowTarget, IContextMenuItem hoveredItem, bool isHeld)
        {
            DrawHelper.SetBeginBatch(windowTarget);

            foreach (var menu in openMenus)
            {
                menu.ContextMenu.RenderBackground();

                foreach (var item in menu.ContextMenu.Items)
                {
                    if (item == hoveredItem)
                    {
                        item.Renderer.Render(menu.ContextMenu.Boundaries.Size, item.Location, isHeld ? MenuItemStatus.Held : MenuItemStatus.Hovered);
                    }
                    else
                    {
                        item.Renderer.Render(menu.ContextMenu.Boundaries.Size, item.Location, MenuItemStatus.Default);
                    }
                }
            }

            DrawHelper.EndBatch();
        }
    }
}
