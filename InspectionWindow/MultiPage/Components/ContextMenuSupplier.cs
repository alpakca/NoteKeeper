using Microsoft.Xna.Framework;
using NoteKeeper.Shell.ContextMenus;
using System;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class ContextMenuSupplier
    {
        private readonly PageActions _pageActions;
        private readonly TabViewport _viewport;
        private readonly ContentQuery _contentQuery;
        private readonly IList<NotePage> _notePages;

        private ContextMenu? _contentMenu;
        private ContextMenu? _pageMenu;

        private int _affectedPageIndex;
        private IPageContent? _affectedContent;

        public ContextMenuSupplier(PageActions pageActions, TabViewport viewport, ContentQuery contentQuery, IList<NotePage> notePages)
        {
            _pageActions = pageActions;
            _viewport = viewport;
            _contentQuery = contentQuery;
            _notePages = notePages;
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
                new ActionContextMenuButton(() => _pageActions.DeletePage(_notePages[_affectedPageIndex], _notePages), null, "Delete Page", null)
            });
        }

        private ContextMenu CreateContentContextMenu()
        {
            return new ContextMenu(new IContextMenuEntry[]
            {
                new ActionContextMenuButton(() => _pageActions.DeleteContent(_notePages[_affectedPageIndex], _affectedContent), null, "Delete Content", null)
            });
        }
    }
}
