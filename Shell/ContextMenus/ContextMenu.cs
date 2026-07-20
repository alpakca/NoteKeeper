using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Elements;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace NoteKeeper.Shell.ContextMenus
{
    internal class ContextMenu
    {
        private const int FrameMargin = 2;

        private Rectangle _boundaries;
        private readonly Texture2D _background;
        private readonly IContextMenuItem[] _items;

        public Rectangle Boundaries { get; }
        public IContextMenuItem[] Items => _items;

        public ContextMenu(IContextMenuItem[] items, Texture2D background)
        {
            _items = items;

            int maxWidth = 0;
            int height = 0;

            foreach (var item in items)
            {
                maxWidth = Math.Max(maxWidth, item.Renderer.MinimumWidth);
                height += item.Renderer.MinimumHeight;
            }

            _boundaries = new Rectangle(0, 0, maxWidth, height);
        }

        public void SetLocation(Point location)
        {
            Point adjusted = location;

            if (location.X + _boundaries.Width + 2 * FrameMargin >= WindowOptions.Width)
            {
                adjusted.X = location.X - _boundaries.Width - 2 * FrameMargin;
            }

            if (location.Y + _boundaries.Height + 2 * FrameMargin >= WindowOptions.Height)
            {
                adjusted.Y = location.Y - _boundaries.Height - 2 * FrameMargin;
            }

            _boundaries = new Rectangle(adjusted, _boundaries.Size);
        }

        public void RenderBackground()
        {
            DrawHelper.Batch.Draw(_background, 
                                  new Rectangle(_boundaries.Location - new Point(FrameMargin), _boundaries.Size + new Point(FrameMargin * 2)), 
                                  Color.White);
        }
    }
}
