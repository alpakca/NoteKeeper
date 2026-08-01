using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.PagedTabs.Serialization
{
    internal static class ContentRegistry
    {
        private static readonly Dictionary<Type, string> _typeToID = new Dictionary<Type, string>();
        private static readonly Dictionary<string, ContentRegistryEntry> _IDToInfo = new Dictionary<string, ContentRegistryEntry>();

        public static void Register<T>(string typeName, Func<PageContentInfo, IPageContent> creator)
        {
            Type type = typeof(T);

            _typeToID[type] = typeName;
            _IDToInfo[typeName] = new ContentRegistryEntry
            {
                TypeName = typeName,
                Creator = creator
            };
        }

        public static ContentRegistryEntry ResolveInfo(string typeID)
        {
            return _IDToInfo[typeID];
        }

        public static string ResolveTypeID(Type type)
        {
            return _typeToID[type];
        }
    }
}
