using NoteKeeper.InspectionWindow;
using NoteKeeper.InspectionWindow.MultiPage;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NoteKeeper.Project
{
    internal static class TabRegistry
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
            if (_creators.TryGetValue(Path.GetExtension(path), out Func<string, Tab> creator))
            {
                return creator(path);
            }

            return null;
        }
    }
}
