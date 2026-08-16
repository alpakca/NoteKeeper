using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using PopUpUI.Input;
using PopUpUI.Toolkit;
using System.Collections.Generic;

#nullable enable

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenuManager
    {
        private const int SubMenuCloseOpenTime = 150;

        private bool isHoveredItemHeld;
        private IContextMenuItem? hoveredItem;

        private float subMenuOpenRemainingTime;
        private bool hoveringSubMenuOpener;

        private readonly ContextMenuRenderer renderer;
        private readonly ContextMenuLayoutCalculator layoutCalculator;
        private readonly LinkedList<SubMenuPair> openMenus;

        public ContextMenuManager()
        {
            openMenus = new LinkedList<SubMenuPair>();

            layoutCalculator = new ContextMenuLayoutCalculator();
            renderer = new ContextMenuRenderer(openMenus, layoutCalculator);
        }

        public void Show(ContextMenu menu, Point mousePosition)
        {
            HideAll();

            layoutCalculator.CalculateContextMenuSize(menu);

            menu.Location = AdjustedMenuTopLeftPosition(mousePosition, menu);
            layoutCalculator.PositionContextMenuItems(menu);

            openMenus.AddLast(new SubMenuPair(null, menu));
        }

        public void HideAll()
        {
            openMenus.Clear();
        }

        public void Update()
        {
            if (openMenus.Count > 1)
            {
                Point mousePosition = Mouse.GetState().Position;
                LinkedListNode<SubMenuPair> peek = openMenus.Last!;

                while (peek != openMenus.First)
                {
                    SubMenuPair pair = peek.Value;
                    if (!InsideContextMenuItem(pair.Button, mousePosition) && 
                        !layoutCalculator.GetContextMenuBackgroundBounds(pair.ContextMenu).Contains(mousePosition))
                    {
                        pair.UpdateCloseTime();
                    }
                    else
                    {
                        pair.DisableCloseTime();
                    }

                    peek = peek.Previous!;
                }

                while (openMenus.Last != openMenus.First)
                {
                    if (!openMenus.Last!.Value.ReadyToClose)
                    {
                        break;
                    }

                    openMenus.RemoveLast();
                }
            }

            if (subMenuOpenRemainingTime > 0 && hoveringSubMenuOpener)
            {
                subMenuOpenRemainingTime -= Time.DeltaTime;
                if (subMenuOpenRemainingTime <= 0)
                {
                    PushContextMenu((SubContextMenuButton)hoveredItem!);
                }
            }
        }

        public void Render()
        {
            renderer.Render(hoveredItem, isHoveredItemHeld);
        }

        public void MousePositionChangeUpdate(PopWindowMouseEventArgs e)
        {
            if (openMenus.Count == 0)
            {
                return;
            }

            HoverUpdate(e);
            HoldUpdate(e);
        }

        public void MouseButtonStateUpdate(PopWindowMouseEventArgs e)
        {
            if (openMenus.Count == 0)
            {
                return;
            }

            HoldUpdate(e);
            SelectUpdate(e);
        }

        private void HoverUpdate(PopWindowMouseEventArgs e)
        {
            LinkedListNode<SubMenuPair> peek = openMenus.Last!;
            while (peek != openMenus.First)
            {
                SubMenuPair pair = peek.Value;
                if (!InsideContextMenuItem(pair.Button, e.MousePosition) && 
                    !layoutCalculator.GetContextMenuBackgroundBounds(pair.ContextMenu).Contains(e.MousePosition))
                {
                    pair.ActiveCloseTime(SubMenuCloseOpenTime);
                }

                peek = peek.Previous!;
            }

            (IContextMenuItem? hovered, SubMenuPair? foundPair) = FindItemOnPosition(e.MousePosition);

            if (hovered != hoveredItem)
            {
                subMenuOpenRemainingTime = SubMenuCloseOpenTime;
            }

            hoveringSubMenuOpener = foundPair == openMenus.Last!.Value && hovered is SubContextMenuButton;
            hoveredItem = hovered;
        }

        private void HoldUpdate(PopWindowMouseEventArgs e)
        {
            isHoveredItemHeld = e.CurrentButtons.HasLeft;
        }

        private void SelectUpdate(PopWindowMouseEventArgs e)
        {
            if (!e.ReleasedButtons.HasLeft)
            {
                return;
            }

            bool anyContextMenuSelected = false;
            foreach (SubMenuPair pair in openMenus)
            {
                if (layoutCalculator.GetContextMenuBackgroundBounds(pair.ContextMenu).Contains(e.MousePosition))
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

            // if selected button is SubContextMenuButton; immediately add it to stack
            if (hoveredItem is SubContextMenuButton subContextMenuButton)
            {
                PushContextMenu(subContextMenuButton);
                return;
            }

            if (hoveredItem != null)
            {
                hoveredItem?.Selected();
                HideAll();
            }
        }

        private void PushContextMenu(SubContextMenuButton contextMenuButton)
        {
            ContextMenu contextMenu = contextMenuButton.SubMenu;
            layoutCalculator.CalculateContextMenuSize(contextMenu);

            contextMenu.Location = GetSubMenuLocation(contextMenuButton);
            layoutCalculator.PositionContextMenuItems(contextMenu);

            openMenus.AddLast(new SubMenuPair(contextMenuButton, contextMenu));
        }

        private Point AdjustedMenuTopLeftPosition(Point topLeftPosition, ContextMenu menu)
        {
            Point location = topLeftPosition;
            Point backgroundSize = layoutCalculator.GetContextMenuBackgroundBounds(menu).Size;

            if (topLeftPosition.X + backgroundSize.X >= WindowOptions.Width)
            {
                location.X -= topLeftPosition.X + backgroundSize.X - WindowOptions.Width;
            }

            if (topLeftPosition.Y + backgroundSize.Y >= WindowOptions.Height)
            {
                location.Y -= topLeftPosition.Y + backgroundSize.Y - WindowOptions.Height;
            }

            return location;
        }

        private Point GetSubMenuLocation(SubContextMenuButton button)
        {
            Point subMenuSize = layoutCalculator.GetContextMenuBackgroundBounds(button.SubMenu).Size;
            if (button.Location.X + button.Size.X + subMenuSize.X < WindowOptions.Width)
            {
                return new Point(button.Location.X + button.Size.X, button.Location.Y);
            }
            else
            {
                return new Point(button.Location.X - subMenuSize.X, button.Location.Y);
            }
        }

        private (IContextMenuItem? Item, SubMenuPair? FoundPair) FindItemOnPosition(Point mousePosition)
        {
            foreach (var pair in openMenus)
            {
                foreach (var entry in pair.ContextMenu.Entries)
                {
                    if (entry is not IContextMenuItem item)
                    {
                        continue;
                    }

                    if (InsideContextMenuItem(item, mousePosition))
                    {
                        return (item, pair);
                    }
                }
            }

            return (null, null);
        }

        private static bool InsideContextMenuItem(IContextMenuItem? item, Point mousePosition)
        {
            if (item == null)
                return false;

            return item.Location.X <= mousePosition.X && mousePosition.X < item.Location.X + item.Size.X &&
                   item.Location.Y <= mousePosition.Y && mousePosition.Y < item.Location.Y + item.Size.Y;
        }
    }

    internal class SubMenuPair
    {
        private bool isClosing = false;
        private float remainingCloseTime;

        public SubContextMenuButton? Button { get; }
        public ContextMenu ContextMenu { get; }

        public bool ReadyToClose => isClosing && remainingCloseTime <= 0;

        public SubMenuPair(SubContextMenuButton? button, ContextMenu contextMenu)
        {
            Button = button;
            ContextMenu = contextMenu;
        }

        public void ActiveCloseTime(float time)
        {
            if (!isClosing)
            {
                isClosing = true;
                remainingCloseTime = time;
            }
        }

        public void DisableCloseTime()
        {
            isClosing = false;
        }

        public void UpdateCloseTime()
        {
            if (isClosing)
            {
                remainingCloseTime -= Time.DeltaTime;
            }
        }
    }
}
