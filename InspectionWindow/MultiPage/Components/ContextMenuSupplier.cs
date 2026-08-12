using Microsoft.Xna.Framework;
using NoteKeeper.InspectionWindow.MultiPage.Properties;
using NoteKeeper.Shell.ContextMenus;
using NoteKeeper.Shell.PropertyExplorer;
using System;
using System.Collections.Generic;

#nullable enable

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class ContextMenuSupplier
    {
        private readonly PageActions _pageActions;
        private readonly TabViewport _viewport;
        private readonly ContentQuery _contentQuery;
        private readonly IList<NotePage> _notePages;

        private readonly Action<IPropertyCollection> _requestPropertyScreen;
        private readonly PageContentProperties _contentProperties;

        private ContextMenu? _contentMenu;
        private ContextMenu? _pageMenu;

        private int _affectedPageIndex;
        private IPageContent? _affectedContent;

        public ContextMenuSupplier(PageActions pageActions, TabViewport viewport, ContentQuery contentQuery, 
                                   Action<IPropertyCollection> requestPropertyScreen, IList<NotePage> notePages)
        {
            _pageActions = pageActions;
            _viewport = viewport;
            _contentQuery = contentQuery;
            _notePages = notePages;

            _requestPropertyScreen = requestPropertyScreen;
            _contentProperties = new PageContentProperties();
        }

        public ContextMenu? GetMenu(Point mousePosition)
        {
            NotePagePosition position = _viewport.ToPagePosition(mousePosition);

            _affectedPageIndex = position.PageIndex;
            _affectedContent = null;

            IPageContent? hit = _contentQuery.HitContent(position);
            if (position.PageIndex != -1 && hit != null)
            {
                _affectedContent = hit;
                return _contentMenu ??= CreateContentContextMenu();
            }

            // didn't hit any pages, hit background
            if (position.PageIndex == -1 ||
                position.PagePosition.X < 0 || position.PagePosition.X > _viewport.EachPageWidth ||
                position.PagePosition.Y < 0 || position.PagePosition.Y > _viewport.EachPageHeight)
            {
                return null;
            }

            return _pageMenu ??= CreatePageContextMenu();
        }

        private ContextMenu CreatePageContextMenu()
        {
            return new ContextMenu(new IContextMenuEntry[]
            {
                new ActionContextMenuButton(() => 
                {
                    _pageActions.DeletePage(_notePages[_affectedPageIndex], _notePages);

                    // update scrolled distance, for case that deleted page was last page & was viewing it
                    _viewport.SetHorizontalScroll(_viewport.ScrolledDistance.X);
                    _viewport.SetVerticalScroll(_viewport.ScrolledDistance.Y);
                }, null, "Delete Page", null)
            });
        }

        private ContextMenu CreateContentContextMenu()
        {
            return new ContextMenu(new IContextMenuEntry[]
            {
                new ActionContextMenuButton(() => _pageActions.DeleteContent(_notePages[_affectedPageIndex], _affectedContent), null, "Delete Content", null),
                new ActionContextMenuButton(() =>
                {
                    _contentProperties.SetContent(_affectedContent);
                    _requestPropertyScreen(_contentProperties);
                }, null, "Properties", null)
            });
        }
    }
}
