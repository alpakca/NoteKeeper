using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.InspectionWindow;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.Shell
{
    internal class TabGroupRenderer
    {
        private const int HorizontalMargin = 2;
        private const int VerticalMargin = 2;
        private const int VisualHeight = 30;

        private readonly SpriteFont defaultTabNameFont;
        private readonly SpriteFont displayedTabNameFont;
        private readonly Texture2D defaultTabBackground;
        private readonly Texture2D displayedTabBackground;

        private readonly Dictionary<Tab, TabVisual> tabVisuals;

        public RenderTarget2D Target { get; private set; }

        public TabGroupRenderer()
        {
            tabVisuals = new Dictionary<Tab, TabVisual>();

            defaultTabNameFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans");
            displayedTabNameFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans");

            defaultTabBackground = Util.GetTexture(1, 1, Color.Black);
            displayedTabBackground = Util.GetTexture(1, 1, Color.MediumPurple);
        }

        public void SizeChanged(Point size)
        {
            Target?.Dispose();
            Target = DrawHelper.CreateRenderTarget(size.X, size.Y);
        }

        public void TabStatuesChanged(ControlPanel panel, int width)
        {
            tabVisuals.Clear();

            int xOffset = 0;
            int yOffset = 0;
            foreach (var tab in panel.ActiveTabs)
            {
                int tabWidth = GetTabWidth(tab, tab == panel.DisplayedTab ? displayedTabNameFont : defaultTabNameFont);
                if (xOffset + tabWidth >= width)
                {
                    yOffset += VisualHeight;
                    xOffset = 0;
                }

                tabVisuals.Add(tab, new TabVisual()
                {
                    Width = tabWidth,
                    XOffset = xOffset,
                    YOffset = yOffset
                });

                xOffset += tabWidth;
            }
        }

        public Rectangle GetTabBoundary(Tab tab)
        {
            TabVisual visual = tabVisuals[tab];
            return new Rectangle(visual.XOffset, visual.YOffset, visual.Width, VisualHeight);
        }

        public int GetTabWidth(Tab tab, bool isDisplayedTab)
        {
            return tabVisuals[tab].Width;
        }

        public Tab HitTab(Point position)
        {
            foreach (var (tab, tabVisual) in tabVisuals)
            {
                if (tabVisual.XOffset <= position.X && position.X < tabVisual.RightPosition &&
                    tabVisual.YOffset <= position.Y && position.Y < tabVisual.YOffset + VisualHeight)
                {
                    return tab;
                }
            }

            return null;
        }

        public void Render(Tab displayedTab)
        {
            DrawHelper.SetBeginBatch(Target);

            foreach (var (tab, tabvisual) in tabVisuals)
            {
                if (tab == displayedTab)
                    DrawTab(tabvisual.XOffset, tabvisual.YOffset, tab, displayedTabNameFont, displayedTabBackground);

                else
                    DrawTab(tabvisual.XOffset, tabvisual.YOffset, tab, defaultTabNameFont, defaultTabBackground);
            }

            DrawHelper.EndBatch();
        }

        private void DrawTab(int xOffset, int yOffset, Tab tab, SpriteFont font, Texture2D background)
        {
            int tabWidth = GetTabWidth(tab, font);
            DrawHelper.Batch.Draw(background,
                              new Rectangle(new Point(xOffset, yOffset), new Point(tabWidth, VisualHeight)),
                              Color.White);

            xOffset += HorizontalMargin;

            DrawHelper.Batch.DrawString(font, tab.Name, new Vector2(xOffset, yOffset), Color.White);
        }

        private int GetTabWidth(Tab tab, SpriteFont tabFont)
        {
            return HorizontalMargin * 2 + (int)tabFont.MeasureString(tab.Name).X;
        }
    }
}
