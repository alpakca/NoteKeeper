using NoteKeeper.InspectionWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class TabCollection : IEnumerable<Tab>
    {
        private readonly List<Tab> _tabs;

        public int Count => _tabs.Count;

        public event Action<TabCollection>? CollectionChanged;

        public TabCollection()
        {
            _tabs = new List<Tab>();
        }

        public Tab this[int index]
        {
            get => _tabs[index];
        }

        public void Add(Tab tab)
        {
            _tabs.Add(tab);
            CollectionChanged?.Invoke(this);
        }

        public void Remove(Tab tab)
        {
            bool closeApproved = true;
            if (tab.TabSerializer.IsModified)
            {
                closeApproved = true; // replace with window call for asking to save changes / discard changes
            }

            if (closeApproved)
            {
                _tabs.Remove(tab);
                CollectionChanged?.Invoke(this);
            }
        }

        public void RemoveAll(IEnumerable<Tab> toRemove)
        {
            List<Tab> modifiedTabs = new List<Tab>();
            foreach (var tab in toRemove)
            {
                if (_tabs.Contains(tab) && tab.TabSerializer.IsModified)
                {
                    modifiedTabs.Add(tab);
                }
            }

            bool closeApproved = true;
            if (modifiedTabs.Count != 0)
            {
                closeApproved = true; // replace with a window call that asks if save changes then close tabs
            }

            if (closeApproved)
            {
                foreach (var tab in toRemove) _tabs.Remove(tab);
                CollectionChanged?.Invoke(this);
            }
        }

        public void Clear()
        {
            List<Tab> modifiedTabs = new List<Tab>();
            foreach (var tab in _tabs)
            {
                if (tab.TabSerializer.IsModified)
                {
                    modifiedTabs.Add(tab);
                }
            }

            bool closeApproved = true;
            if (modifiedTabs.Count != 0)
            {
                closeApproved = true; // replace with window call
            }

            if (closeApproved)
            {
                _tabs.Clear();
                CollectionChanged?.Invoke(this);
            }
        }

        public int IndexOf(Tab tab)
        {
            return _tabs.IndexOf(tab);
        }

        public Tab? Find(Predicate<Tab> predicate)
        {
            return _tabs.Find(predicate);
        }

        public bool Contains(Tab tab)
        {
            return _tabs.Contains(tab);
        }

        public IEnumerator<Tab> GetEnumerator()
        {
            return _tabs.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return GetEnumerator();
        }
    }
}
