using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using NoteKeeper.Shell.ContextMenus;
using PopUpUI.Input;
using PopUpUI.Surface;
using PopUpUI.Toolkit;
using System;
using System.Collections.ObjectModel;
using System.Diagnostics;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class MultiPageTab : PageContentTab
    {
        private readonly TabViewport viewport;
        private readonly TabWindow window;

        public override ITabViewport TabViewport => viewport;
        public override PopWindow Window => window;

        public ObservableCollection<NotePage> NotePages { get; }

        public MultiPageTab(Point size)
        {
            NotePages = new ObservableCollection<NotePage>();

            viewport = new TabViewport { UISize = size };
            window = new TabWindow(size, viewport, NotePages);

            NotePages.CollectionChanged += (sender, _) => viewport.PageCount = ((ObservableCollection<NotePage>)sender).Count;
        }

        public override void AddContent(IPageContent content)
        {
            // UISize / 2 gives the center of screen, in mousePosition units, which is the focused part of the page
            (int focusedPageIndex, Point focusedPageViewedCenter) = viewport.ToPagePosition(viewport.UISize / new Point(2));

            Point location = focusedPageViewedCenter - content.Size / new Point(2);
            location -= new Point(0, 50); // to make it appear at eye-sight

            location.X = Math.Max(location.X, 50);
            location.Y = Math.Max(location.Y, 50);

            content.Location = location;
            NotePages[focusedPageIndex].Contents.Add(content);
        }

        public override Point GetContentScreenLocation(IPageContent content)
        {
            int contentPageIndex = -1;
            for (int i = 0; i < NotePages.Count; i++)
            {
                if (NotePages[i].Contents.Contains(content))
                {
                    contentPageIndex = i;
                    break;
                }
            }

            if (contentPageIndex == -1)
            {
                throw new ArgumentException("Content does not exist in any tab.");
            }

            return viewport.FirstPageStartPosition + contentPageIndex * viewport.MovingDimensionPageStep.Times(viewport.Zoom) + content.Location.Times(viewport.Zoom);
        }

        public override void ShellModeUpdated(ShellModes mode)
        {
            window.ModeUpdated(mode);
        }

        public override void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e)
        {
            window.SimulateMouseButtonStateInput(e);
        }

        public override ContextMenu? GetContextMenu(Point mousePosition)
        {
            return new ContextMenu([
            new ActionContextMenuButton(() => { })
            {
                RightText = "Ctrl + F",
                RightTextColor = Color.Gray,
                RightTextFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans"),
                RightTextHorizontalPadding = 4
            },
            new SubContextMenuButton(new ContextMenu([
                new ActionContextMenuButton(() => { Trace.WriteLine("yep"); })
                {
                    RightText = "Ctrl + F",
                    RightTextColor = Color.Gray,
                    RightTextFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans"),
                    RightTextHorizontalPadding = 4
                },
                new SubContextMenuButton(new ContextMenu([
                new ActionContextMenuButton(() => { })
                {
                    RightText = "Ctrl + F",
                    RightTextColor = Color.Gray,
                    RightTextFont = ContentAssistant.FromLoaded<SpriteFont>("Font_ComicSans"),
                    RightTextHorizontalPadding = 4
                }]){Background = Util.GetTexture(1, 1, Color.Tomato)}){ RightIcon = Util.GetTexture(1, 1, Color.Aqua) }]){Background = Util.GetTexture(1, 1, Color.Tomato)})
            {
                RightIcon = Util.GetTexture(1, 1, Color.Aqua)
            }])
            {
                Background = Util.GetTexture(1, 1, Color.Tomato)
            };
        }
    }
}
