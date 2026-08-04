using System;
using System.Collections.Generic;
using System.Text;

namespace NoteKeeper.InspectionWindow.MultiPage.Serialization
{
    internal class ContentRegistryEntry
    {
        public string TypeName { get; init; }
        public Type InfoType { get; init; }
        public Func<PageContentInfo, IPageContent> Creator { get; init; }
    }
}
