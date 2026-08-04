using System;
using System.Collections.Generic;

namespace NoteKeeper.InspectionWindow.MultiPage
{
    internal class ContentCollection
    {
        private readonly List<IPageContent> _contents;

        public int Count => _contents.Count;

        public IEnumerable<IPageContent> DrawOrder => _contents;
        public IEnumerable<IPageContent> HitOrder
        {
            get
            {
                for (int i = _contents.Count - 1; i >= 0; i--)
                {
                    yield return _contents[i];
                }
            }
        }

        public ContentCollection()
        {
            _contents = new List<IPageContent>();
        }

        public bool Contains(IPageContent content)
        {
            return _contents.Contains(content);
        }

        public bool Remove(IPageContent content)
        {
            return _contents.Remove(content);
        }

        public int GetContentIndex(IPageContent content)
        {
            return _contents.IndexOf(content);
        }

        public void InsertContent(int index, IPageContent content)
        {
            _contents.Insert(index, content);
        }

        public void RemoveContentAt(int index)
        {
            _contents.RemoveAt(index);
        }

        public void PushFront(IPageContent content)
        {
            _contents.Add(content);
        }

        public void PushFrontRange(IEnumerable<IPageContent> contents)
        {
            _contents.AddRange(contents);
        }

        public void PushFrontRange(params ReadOnlySpan<IPageContent> contents)
        {
            _contents.AddRange(contents);
        }

        public void PushBack(IPageContent content)
        {
            _contents.Insert(0, content);
        }

        public void RemoveFromFront()
        {
            _contents.RemoveAt(_contents.Count - 1);
        }

        public void RemoveFromFrontRange(int count)
        {
            _contents.RemoveRange(_contents.Count - count, count);
        }

        public void RemoveFromBack()
        {
            _contents.RemoveAt(0);
        }

        public void RemoveFromBackRange(int count)
        {
            _contents.RemoveRange(0, count);
        }

        public void BringToFront(IPageContent content)
        {
            if (_contents.Remove(content))
            {
                PushFront(content);
            }
        }

        public void SendToBack(IPageContent content)
        {
            if (_contents.Remove(content))
            {
                PushBack(content);
            }
        }

        public void Swap(IPageContent first, IPageContent second)
        {
            int firstIndex = _contents.IndexOf(first);
            int secondIndex = _contents.IndexOf(second);

            if (firstIndex == -1 || secondIndex == -1)
                throw new ArgumentException("Content(s) does not exist in collection");

            SwapByIndex(firstIndex, secondIndex);
        }

        public void MoveForward(IPageContent content)
        {
            int index = _contents.IndexOf(content);
            if (index >= 0 && index < _contents.Count - 1)
            {
                SwapByIndex(index, index + 1);
            }
        }

        public void MoveBackward(IPageContent content)
        {
            int index = _contents.IndexOf(content);
            if (index >= 0 && index < _contents.Count - 1)
            {
                SwapByIndex(index, index - 1);
            }
        }

        public void MoveAbove(IPageContent content, IPageContent reference)
        {
            if (ReferenceEquals(content, reference))
            {
                return;
            }

            int refIndex = _contents.IndexOf(reference);
            if (refIndex == -1)
            {
                throw new ArgumentException("Reference content does not exist in collection");
            }

            if (!_contents.Remove(content))
            {
                return;
            }

            _contents.Insert(refIndex + 1, content);
        }

        public void MoveBelow(IPageContent content, IPageContent reference)
        {
            if (ReferenceEquals(content, reference))
            {
                return;
            }

            int refIndex = _contents.IndexOf(reference);
            if (refIndex == -1)
            {
                throw new ArgumentException("Reference content does not exist in collection");
            }

            if (!_contents.Remove(content))
            {
                return;
            }

            _contents.Insert(refIndex, content);
        }

        private void SwapByIndex(int first, int second)
        {
            (_contents[first], _contents[second]) = (_contents[second], _contents[first]);
        }
    }
}
