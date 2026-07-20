using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Input;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenuManager
    {
        private const int SubMenuCloseOpenTime = 150;

        private bool isHoveredItemHeld;
        private IContextMenuItem? hoveredItem;

        private float subMenuOpenRemainingTime;

        private readonly ContextMenuRenderer renderer;
        private readonly LinkedList<SubMenuEntry> openMenus;

        public ContextMenuManager()
        {
            openMenus = new LinkedList<SubMenuEntry>();
            renderer = new ContextMenuRenderer(openMenus);
        }

        public void Show(ContextMenu menu)
        {
            HideAll();
            openMenus.AddLast(new SubMenuEntry(null, menu));
        }

        public void HideAll()
        {
            openMenus.Clear();
        }

        public void Update()
        {
            if (openMenus.Count <= 1)
            {
                return;
            }

            LinkedListNode<SubMenuEntry> entry = openMenus.First!.Next!;
            for (int i = 1; i < openMenus.Count; i++)
            {
                if (entry.Value.CanBeClosedUpdate())
                {
                    LinkedListNode<SubMenuEntry> toRemove = entry;
                    entry = entry.Next!;

                    openMenus.Remove(toRemove);
                }
            }
        }

        public void HoverUpdate(PopWindowMouseEventArgs e)
        {
            // if mouse is NOT inside openMenus.Peek at somewhere OR at the button that opened it;
            // pop it from the stack

            // if mouse is on a SubContextMenuButton AND its timer goes off (so enough time past)
            // push its submenu to stack

            if (openMenus.Count == 0)
            {
                return;
            }

            while (openMenus.Count > 1) // always remain the root context button
            {
                SubMenuEntry peek = openMenus.Last!.Value;
                if (InsideContextMenuItem(peek.Button, e.MousePosition) || peek.ContextMenu.Boundaries.Contains(e.MousePosition))
                {
                    break;
                }

                peek.ActiveCloseTime(SubMenuCloseOpenTime);
            }

            IContextMenuItem? hovered = FindItemOnPosition(e.MousePosition);
            
            if (hoveredItem is SubContextMenuButton button)
            {
                if (hovered == hoveredItem)
                {
                    subMenuOpenRemainingTime -= Time.DeltaTime;
                    if (subMenuOpenRemainingTime <= 0)
                    {
                        openMenus.AddLast(new SubMenuEntry(button, button.SubMenu));
                    }
                }
                else
                {
                    subMenuOpenRemainingTime = SubMenuCloseOpenTime;
                }
            }

            hoveredItem = hovered;
        }

        private IContextMenuItem? FindItemOnPosition(Point mousePosition)
        {
            foreach (var entry in openMenus)
            {
                foreach (var item in entry.ContextMenu.Items)
                {
                    if (InsideContextMenuItem(item, mousePosition))
                    {
                        return item;
                    }
                }
            }

            return null;
        }

        public void HoldUpdate(PopWindowMouseEventArgs e)
        {
            if (openMenus.Count == 0)
            {
                return;
            }

            isHoveredItemHeld = e.CurrentButtons.HasLeft;
        }

        public void SelectUpdate(PopWindowMouseEventArgs e)
        {
            if (openMenus.Count == 0)
            {
                return;
            }

            bool anyContextMenuSelected = false;
            foreach (var entry in openMenus)
            {
                if (entry.ContextMenu.Boundaries.Contains(e.MousePosition))
                {
                    anyContextMenuSelected = true;
                    break;
                }
            }

            if (!anyContextMenuSelected)
            {
                HideAll();
                return;
            }

            hoveredItem?.Selected();

            // if selected button is SubContextMenuButton; immediately add it to stack
            if (hoveredItem is SubContextMenuButton contextMenuButton)
            {
                openMenus.AddLast(new SubMenuEntry(contextMenuButton, contextMenuButton.SubMenu));
            }
        }

        public void RenderUpdate(RenderTarget2D windowTexture)
        {
            renderer.Render(windowTexture, hoveredItem, isHoveredItemHeld);
        }

        private static bool InsideContextMenuItem(IContextMenuItem? item, Point mousePosition)
        {
            if (item == null)
                return false;

            return item.Location.X <= mousePosition.X && mousePosition.X < item.Location.X + item.Size.X &&
                   item.Location.Y <= mousePosition.Y && mousePosition.Y < item.Location.Y + item.Size.Y;
        }
    }

    internal class SubMenuEntry
    {
        private bool isClosing = false;
        private float remainingCloseTime;

        public SubContextMenuButton? Button { get; }
        public ContextMenu ContextMenu { get; }

        public SubMenuEntry(SubContextMenuButton? button, ContextMenu contextMenu)
        {
            Button = button;
            ContextMenu = contextMenu;
        }

        public void ActiveCloseTime(float time)
        {
            isClosing = true;
            remainingCloseTime = time;
        }

        public bool CanBeClosedUpdate()
        {
            if (isClosing)
            {
                remainingCloseTime -= Time.DeltaTime;

                if (remainingCloseTime <= 0)
                {
                    return true;
                }
            }

            return false;
        }
    }
}
