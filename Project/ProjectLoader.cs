using NoteKeeper.InspectionWindow;
using NoteKeeper.Shell;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace NoteKeeper.Project
{
    internal class ProjectLoader
    {
        private const string NoteProjectExtension = ".note";

        private readonly string[] _args;
        private readonly LoadedState _state;

        public LoadedState State => _state;

        public ProjectLoader(string[] args)
        {
            if (args.Length == 0)
            {
                _state = LoadedState.CreatorHub;
            }
            else if (args.Length == 1 && Path.GetExtension(args[0]) == NoteProjectExtension)
            {
                _state = LoadedState.Project;
            }
            else
            {
                _state = LoadedState.SingularTabs;
            }

            _args = args;
#if DEBUG
            _state = LoadedState.SingularTabs;
            _args = [@"C:\Users\user\Desktop\MultiPageTest.mlpage"];
#endif
        }

        public List<Tab> Load()
        {
            List<Tab> tabs = new List<Tab>();
            if (_state == LoadedState.Project)
            {
                using StreamReader reader = new StreamReader(_args[0]);

                while (!reader.EndOfStream)
                {
                    string filePath = reader.ReadLine();
                    Tab? tab = TabRegistry.Create(filePath);

                    if (tab != null)
                    {
                        tabs.Add(tab);
                    }
                }
            }
            else if (_state == LoadedState.SingularTabs)
            {
                foreach (var filePath in _args)
                {
                    Tab? tab = TabRegistry.Create(filePath);
                    if (tab != null)
                    {
                        tabs.Add(tab);
                    }
                }
            }

            return tabs;
        }

        public void Save(IList<Tab> openTabs)
        {
            if (_state == LoadedState.Project)
            {
                using StreamWriter writer = new StreamWriter(_args[0]);
                foreach (var tab in openTabs)
                {
                    writer.WriteLine(tab.SourcePath);
                }
            }
            else if (_state == LoadedState.SingularTabs)
            {
                foreach (var tab in openTabs)
                {
                    tab.TabSerializer.Save();
                }
            }
        }
    }
}
