using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class PageSwapSession
    {
        private int _initialSwapPageIndex;
        private int _currentPageIndex;
        private bool _isSwapping;

        private readonly PageActions _pageActions;
        private readonly IList<NotePage> _notePages;

        public int SwappingPageIndex => _currentPageIndex;
        public int PagePositionOffset { get; }

        public PageSwapSession(PageActions pageActions, IList<NotePage> notePages)
        {
            _pageActions = pageActions;
            _notePages = notePages;
        }

        public void HoldStart(NotePagePosition position)
        {
            if (_isSwapping)
                return;

            _initialSwapPageIndex = position.PageIndex;
            _currentPageIndex = position.PageIndex;

            _isSwapping = true;
        }

        public void HoldUpdate(NotePagePosition position)
        {
            if (!_isSwapping)
                return; 

            if (position.PageIndex != _currentPageIndex)
            {
                (_notePages[position.PageIndex], _notePages[_currentPageIndex]) = (_notePages[_currentPageIndex], _notePages[position.PageIndex]);
                _currentPageIndex = position.PageIndex;
            }
        }

        public void HoldEnded(NotePagePosition position)
        {
            if (!_isSwapping)
                return;

            if (position.PageIndex == _initialSwapPageIndex)
            {
                return;
            }

            _isSwapping = false;

            // _pageActions.SwappedPages();
        }
    }
}
