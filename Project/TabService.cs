using NoteKeeper.InspectionWindow;
using NoteKeeper.InspectionWindow.MultiPage;
using NoteKeeper.Shell.ChildWindow;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NoteKeeper.Project
{
    internal static class TabService
    {
        private static readonly Dictionary<string, Func<string, Tab>> _creators = new Dictionary<string, Func<string, Tab>>();

        [ModuleInitializer]
        public static void InitializeRegistry()
        {
            Register<MultiPageTab>(MultiPageTab.Creator);
        }

        public static void Register<T>(Func<string, Tab> creator) where T : Tab
        {
            string extension = typeof(T).GetCustomAttribute<TabFileExtensionAttribute>()?.Extension
                ?? throw new Exception();

            _creators.Add(extension, creator);
        }

        public static Tab? Create(string path)
        {
            TabSerializationResult result = TabSerializationResult.WrongFileFormat;
            if (_creators.TryGetValue(Path.GetExtension(path), out Func<string, Tab> creator))
            {
                Tab created = creator(path);
                result = created.TabSerializer.Load();

                if (result == TabSerializationResult.Success)
                {
                    return created;
                }
            }

            ShowSerializationResultMessage(result, path);
            return null;
        }

        private static void ShowSerializationResultMessage(TabSerializationResult result, string filePath)
        {
            switch (result)
            {
                case TabSerializationResult.WrongFileFormat:
                    MessageWindow.Show($"Couldn't open the file: {filePath}.\nUnknown file format or file structure is corrupted.");
                    break;

                case TabSerializationResult.FileNotFound:
                    MessageWindow.Show($"No file found at path: {filePath}. Please try again.");
                    break;

                case TabSerializationResult.IOError:
                    MessageWindow.Show($"An IO error occured while loading the file. Please try again. File path: {filePath}.");
                    break;
            }
        }

        /// <summary>Tries to save given tabs to their source paths. Shows a message window for tabs that failed to be saved.</summary>
        /// <param name="tabs">Tabs to save.</param>
        /// <returns>List of tabs that successfully saved to their source paths.</returns>
        public static List<Tab> SaveTabs(IEnumerable<Tab> tabs)
        {
            List<string> unsuccessfulTabPaths = new List<string>();
            List<Tab> successfulTabs = new List<Tab>();

            foreach (var tab in tabs)
            {
                TabSerializationResult result = tab.TabSerializer.Save();
                if (result == TabSerializationResult.Success)
                {
                    successfulTabs.Add(tab);
                    continue;
                }

                unsuccessfulTabPaths.Add(tab.SourcePath);
            }

            if (unsuccessfulTabPaths.Count > 0)
            {
                MessageWindow.Show($"An IO error occured while saving following tabs, please try again: "
                                 + string.Join("\n\t", unsuccessfulTabPaths));
            }

            return successfulTabs;
        }
    }
}
