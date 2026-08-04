using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace NoteKeeper.Project
{
    internal static class ProjectLoader
    {
        public static List<Tab> Load(string[] args, IList<Tab> alreadyLoadedTabs)
        {
            List<Tab> tabs = new List<Tab>();
            foreach (var filePath in args)
            {
                Tab? tab = TabRegistry.Create(filePath);
                if (tab != null && !alreadyLoadedTabs.Any(loadedTab => loadedTab.SourcePath == tab.SourcePath))
                {
                    tabs.Add(tab);
                }
            }

            return tabs;
        }

        public static void Save(IList<Tab> openTabs)
        {
            foreach (var tab in openTabs)
            {
                tab.TabSerializer.Save();
            }
        }
    }
}
