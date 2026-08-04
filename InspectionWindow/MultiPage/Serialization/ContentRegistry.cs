using System;
using System.Collections.Generic;
using System.Reflection;
using System.Runtime.CompilerServices;

namespace NoteKeeper.InspectionWindow.MultiPage.Serialization
{
    internal static class ContentRegistry
    {
        private static readonly Dictionary<Type, string> _typeToID = new Dictionary<Type, string>();
        private static readonly Dictionary<string, ContentRegistryEntry> _IDToInfo = new Dictionary<string, ContentRegistryEntry>();

        [ModuleInitializer]
        public static void InitializeRegistry()
        {
            Register<TextContentInfo>(PageTextContent.Create);
            Register<ImageContentInfo>(PageImageContent.Create);
        }

        public static void Register<T>(Func<PageContentInfo, IPageContent> creator)
        {
            Type type = typeof(T);
            string typeName = type.GetCustomAttribute<ContentTypeNameAttribute>()?.TypeName
                ?? throw new PageSerializationException("Registering type requires ContentTypeNameAttribute.");

            _typeToID[type] = typeName;
            _IDToInfo[typeName] = new ContentRegistryEntry
            {
                TypeName = typeName,
                InfoType = type,
                Creator = creator
            };
        }

        public static ContentRegistryEntry ResolveInfo(string typeID)
        {
            return _IDToInfo[typeID];
        }

        public static ContentRegistryEntry ResolveInfoByType(Type type)
        {
            return _IDToInfo[_typeToID[type]];
        }

        public static string ResolveTypeID(Type type)
        {
            return _typeToID[type];
        }
    }
}
