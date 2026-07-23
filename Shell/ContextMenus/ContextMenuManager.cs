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
        private readonly MenuLayoutCalculator layoutCalculator;
        private readonly LinkedList<SubMenuEntry> openMenus;

        public MenuItemStyleProvider StyleProvider { get; }

        public ContextMenuManager()
        {
            StyleProvider = new MenuItemStyleProvider();
            openMenus = new LinkedList<SubMenuEntry>();

            layoutCalculator = new MenuLayoutCalculator(StyleProvider);
            renderer = new ContextMenuRenderer(openMenus, StyleProvider, layoutCalculator);
        }

        public void Show(ContextMenu menu, Point mousePosition)
        {
            HideAll();

            layoutCalculator.CalculateContextMenuSize(menu);

            menu.Location = AdjustedMenuTopLeftPosition(mousePosition, menu);
            layoutCalculator.PositionContextMenuItems(menu);

            openMenus.AddLast(new SubMenuEntry(null, menu));
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
                LinkedListNode<SubMenuEntry> peek = openMenus.Last!;

                while (peek != openMenus.First)
                {
                    if (!InsideContextMenuItem(peek.Value.Button, mousePosition) && !peek.Value.ContextMenu.InsideBoundaries(mousePosition))
                    {
                        peek.Value.UpdateCloseTime();
                    }
                    else
                    {
                        peek.Value.DisableCloseTime();
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

        public void RenderUpdate(RenderTarget2D windowTexture)
        {
            renderer.Render(windowTexture, hoveredItem, isHoveredItemHeld);
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
            LinkedListNode<SubMenuEntry> peek = openMenus.Last!;
            while (peek != openMenus.First)
            {
                SubMenuEntry value = peek.Value;
                if (!InsideContextMenuItem(value.Button, e.MousePosition) && !value.ContextMenu.InsideBoundaries(e.MousePosition))
                {
                    value.ActiveCloseTime(SubMenuCloseOpenTime);
                }

                peek = peek.Previous!;
            }

            (IContextMenuItem? hovered, SubMenuEntry? foundEntry) = FindItemOnPosition(e.MousePosition);

            if (hovered != hoveredItem)
            {
                subMenuOpenRemainingTime = SubMenuCloseOpenTime;
            }

            hoveringSubMenuOpener = foundEntry == openMenus.Last!.Value && hovered is SubContextMenuButton;
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
            foreach (var entry in openMenus)
            {
                if (entry.ContextMenu.InsideBoundaries(e.MousePosition))
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

            contextMenu.Location = AdjustedMenuTopLeftPosition(contextMenuButton.GetPossibleSubMenuPlacement(), contextMenu);
            layoutCalculator.PositionContextMenuItems(contextMenu);

            openMenus.AddLast(new SubMenuEntry(contextMenuButton, contextMenu));
        }

        private static Point AdjustedMenuTopLeftPosition(Point topLeftPosition, ContextMenu menu)
        {
            Point location = topLeftPosition;
            if (topLeftPosition.X + menu.FramedSize.X >= WindowOptions.Width)
            {
                location.X -= topLeftPosition.X + menu.FramedSize.X - WindowOptions.Width;
            }

            if (topLeftPosition.Y + menu.FramedSize.Y >= WindowOptions.Height)
            {
                location.Y -= topLeftPosition.Y + menu.FramedSize.Y - WindowOptions.Height;
            }

            return location;
        }

        private (IContextMenuItem? Item, SubMenuEntry? FoundEntry) FindItemOnPosition(Point mousePosition)
        {
            foreach (var entry in openMenus)
            {
                foreach (var item in entry.ContextMenu.Items)
                {
                    if (InsideContextMenuItem(item, mousePosition))
                    {
                        return (item, entry);
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

    internal class SubMenuEntry
    {
        private bool isClosing = false;
        private float remainingCloseTime;

        public SubContextMenuButton? Button { get; }
        public ContextMenu ContextMenu { get; }

        public bool ReadyToClose => isClosing && remainingCloseTime <= 0;

        public SubMenuEntry(SubContextMenuButton? button, ContextMenu contextMenu)
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
