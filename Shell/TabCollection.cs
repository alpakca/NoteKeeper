using NoteKeeper.InspectionWindow;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

#nullable enable

namespace NoteKeeper.Shell
{
    internal class TabCollection : IEnumerable<Tab>
    {
        private readonly List<Tab> _tabs;

        public int Count => _tabs.Count;
        public bool HasModifiedTabs => _tabs.Any(tab => tab.TabHistory.IsModified);

        public event Action<TabCollection>? TabCountChanged;
        public event Action<IEnumerable<Tab>>? TabRemovalRequested;

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
            TabCountChanged?.Invoke(this);
        }

        public void RemoveWithAskingSave(Tab tab)
        {
            if (tab.TabHistory.IsModified)
            {
                TabRemovalRequested?.Invoke([tab]);
            }
            else
            {
                _tabs.Remove(tab);
                TabCountChanged?.Invoke(this);
            }
        }

        public void RemoveAllWithAskingSave(IEnumerable<Tab> toRemove)
        {
            List<Tab> modifiedTabs = new List<Tab>();
            foreach (var tab in toRemove)
            {
                if (_tabs.Contains(tab) && tab.TabHistory.IsModified)
                {
                    modifiedTabs.Add(tab);
                }
            }

            if (modifiedTabs.Count != 0)
            {
                TabRemovalRequested?.Invoke(modifiedTabs);
            }
            else
            {
                RemoveTabs(toRemove);
            }
        }

        public void ClearWithAskingSave()
        {
            List<Tab> modifiedTabs = new List<Tab>();
            foreach (var tab in _tabs)
            {
                if (tab.TabHistory.IsModified)
                {
                    modifiedTabs.Add(tab);
                }
            }

            if (modifiedTabs.Count != 0)
            {
                TabRemovalRequested?.Invoke(modifiedTabs);
            }
            else
            {
                _tabs.Clear();
                TabCountChanged?.Invoke(this);
            }
        }

        public void RemoveTabs(IEnumerable<Tab> source)
        {
            foreach (var tab in source)
            {
                _tabs.Remove(tab);
            }

            TabCountChanged?.Invoke(this);
        }

        public void Swap(int firstTabIndex, int secondTabIndex)
        {
            (_tabs[firstTabIndex], _tabs[secondTabIndex]) = (_tabs[secondTabIndex], _tabs[firstTabIndex]);
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
