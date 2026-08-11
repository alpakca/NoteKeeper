using Microsoft.Xna.Framework;
using System.Collections.Generic;

#nullable enable

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class ContentQuery
    {
        private readonly IList<NotePage> _notePages;

        public ContentQuery(IList<NotePage> notePages)
        {
            _notePages = notePages;
        }

        public IPageContent? HitContent(NotePagePosition position) 
        {
            (int pageIndex, Point pagePosition) = position;
            if (pageIndex == -1)
                return null;

            foreach (var content in _notePages[pageIndex].Contents.HitOrder)
            {
                Rectangle normalized = PageContentUtil.Normalized(new Rectangle(content.Location, content.Size));

                if (normalized.X <= pagePosition.X && pagePosition.X < normalized.Right &&
                    normalized.Y <= pagePosition.Y && pagePosition.Y < normalized.Bottom)
                {
                    return content;
                }
            }

            return null;
        }

        public int PageIndexOf(IPageContent content)
        {
            for (int i = 0; i < _notePages.Count; i++)
            {
                if (_notePages[i].Contents.Contains(content))
                {
                    return i;
                }
            }

            return -1;
        }

        public IEnumerable<IPageContent> HitOrder(int pageIndex)
        {
            return _notePages[pageIndex].Contents.HitOrder;
        }

        public IEnumerable<IPageContent> DrawOrder(int pageIndex)
        {
            return _notePages[pageIndex].Contents.DrawOrder;
        }
    }
}
