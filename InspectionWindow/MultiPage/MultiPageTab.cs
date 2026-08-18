using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow.MultiPage.Components;
using NoteKeeper.Project;
using NoteKeeper.Shell.ContextMenus;
using NoteKeeper.Shell.ToolBar;
using PopUpUI.Input;
using PopUpUI.Surface;
using System;
using System.Collections.ObjectModel;

#nullable enable

namespace NoteKeeper.InspectionWindow.MultiPage
{
    [TabFile("Multi-Paged Note", ".mlpage")]
    internal class MultiPageTab : Tab
    {
        private readonly TabViewport viewport;
        private readonly TabWindow window;

        private readonly PageActions pageActions;
        private readonly ContentQuery contentQuery;
        private readonly ContextMenuSupplier menuSupplier;

        public override ITabSerializer TabSerializer { get; }
        public override ITabViewport TabViewport => viewport;
        public override TabHistory TabHistory { get; }
        public override PopWindow Window => window;

        public override IToolMenu? ToolMenu { get; }

        public ObservableCollection<NotePage> NotePages { get; }

        public MultiPageTab(string sourcePath) : base(sourcePath)
        {
            NotePages = new ObservableCollection<NotePage>();
            TabHistory = new TabHistory();

            pageActions = new PageActions(TabHistory);
            viewport = new TabViewport();

            TabToolMenu toolMenu = new TabToolMenu(this, pageActions);
            ToolMenu = toolMenu;
            TabSerializer = new TabSerializer(this);

            contentQuery = new ContentQuery(NotePages);
            window = new TabWindow(pageActions, viewport, contentQuery, toolMenu, NotePages);
            menuSupplier = new ContextMenuSupplier(pageActions, viewport, contentQuery, RequestPropertyScreenOpen, NotePages);

            NotePages.CollectionChanged += (sender, _) => viewport.PageCount = ((ObservableCollection<NotePage>)sender!).Count;
        }

        public void AddContent(IPageContent content)
        {
            // UISize / 2 gives the center of screen, in mousePosition units, which is the focused part of the page
            (int focusedPageIndex, Point focusedPageViewedCenter) = viewport.ToPagePosition(viewport.UISize / new Point(2));

            Point location = focusedPageViewedCenter - content.Size / new Point(2);
            location -= new Point(0, 50); // to make it appear at eye-sight

            location.X = Math.Max(location.X, 50);
            location.Y = Math.Max(location.Y, 50);

            content.SetLocation(location);
            content.SetSize(new Point(200, 50));

            pageActions.AddContent(NotePages[focusedPageIndex], content);
        }

        public Point GetContentScreenLocation(IPageContent content)
        {
            int contentPageIndex = contentQuery.PageIndexOf(content);
            if (contentPageIndex == -1)
            {
                throw new ArgumentException("Content does not exist in any tab.");
            }

            return viewport.FirstPageStartPosition + contentPageIndex * viewport.MovingDimensionPageStep.Times(viewport.Zoom) + content.Location.Times(viewport.Zoom);
        }

        public override void SimulateMouseButtonStateInput(PopWindowMouseEventArgs e)
        {
            window.SimulateMouseButtonStateInput(e);
        }

        public override ContextMenu? GetContextMenu(Point mousePosition)
        {
            return menuSupplier.GetMenu(mousePosition);
        }

        public static MultiPageTab Creator(string path)
        {
            return new MultiPageTab(path);
        }
    }
}
