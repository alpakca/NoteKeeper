using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell.ContextMenus
{
    internal class SubMenuButtonRenderer : IContextMenuItemRenderer
    {
        private const int IconMaxMargin = 4;
        private const int TextRightSizeMargin = 20;
        private const int IconLength = 32;

        private readonly string _text;
        private readonly Texture2D? _icon;
        private readonly SpriteFont _font;
        private readonly Texture2D _arrowTexture;
        private readonly Texture2D _background;
        private readonly Texture2D _hoverOverlay;
        private readonly Texture2D _heldOverlay;

        private readonly int textWidth;

        public int MinimumWidth { get; }
        public int MinimumHeight { get; }

        public SubMenuButtonRenderer(Texture2D? icon, string text, SpriteFont font, Texture2D arrowTexture,
                                     Texture2D background, Texture2D hoverOverlay, Texture2D heldOverlay)
        {
            _icon = icon;
            _font = font;
            _text = text;
            _arrowTexture = arrowTexture;
            _background = background;
            _hoverOverlay = hoverOverlay;
            _heldOverlay = heldOverlay;

            textWidth = (int)font.MeasureString(text).X;

            Point minimumSize = GetMinimumSize();
            MinimumWidth = minimumSize.X;
            MinimumHeight = minimumSize.Y;
        }

        public void Render(Point totalSize, Point location, MenuItemStatus status)
        {
            DrawHelper.Batch.Draw(_background, new Rectangle(location, totalSize), Color.White);

            int xOffset = location.X + IconMaxMargin;
            int yOffset = location.Y + IconMaxMargin;

            DrawHelper.Batch.Draw(_icon, new Rectangle(xOffset, yOffset, IconLength, IconLength), Color.White);
            xOffset += IconLength + IconMaxMargin;

            DrawHelper.Batch.DrawString(_font, _text, new Vector2(xOffset, yOffset), Color.White);
            xOffset += totalSize.X - IconLength - IconMaxMargin;

            DrawHelper.Batch.Draw(_arrowTexture, new Rectangle(xOffset, yOffset, IconLength, IconLength), Color.White);

            if (status == MenuItemStatus.Hovered)
                DrawHelper.Batch.Draw(_hoverOverlay, new Rectangle(location, totalSize), Color.White);

            else if (status == MenuItemStatus.Held)
                DrawHelper.Batch.Draw(_heldOverlay, new Rectangle(location, totalSize), Color.White);
        }

        public Point GetMinimumSize()
        {
            return new Point(IconMaxMargin * 2 + IconLength + textWidth + TextRightSizeMargin + IconMaxMargin * 2 + IconLength,
                             IconMaxMargin * 2 + _font.LineSpacing);
        }
    }
}
