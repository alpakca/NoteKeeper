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
                Swap(position.PageIndex, _currentPageIndex);
                _currentPageIndex = position.PageIndex;
            }
        }

        public void HoldEnded(NotePagePosition position)
        {
            if (!_isSwapping)
                return;

            if (position.PageIndex == _initialSwapPageIndex)
            {
                _isSwapping = false;
                return;
            }

            // undo swapping done in HoldUpdate since running page actions will swap two pages again when Run() called
            Swap(position.PageIndex, _initialSwapPageIndex);

            _pageActions.SwapPages(position.PageIndex, _initialSwapPageIndex, _notePages);
            _isSwapping = false;
        }

        private void Swap(int firstIndex, int secondIndex)
        {
            (_notePages[firstIndex], _notePages[secondIndex]) = (_notePages[secondIndex], _notePages[firstIndex]);
        }
    }
}
