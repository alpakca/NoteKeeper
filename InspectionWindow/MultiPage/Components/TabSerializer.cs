using NoteKeeper.InspectionWindow.MultiPage.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace NoteKeeper.InspectionWindow.MultiPage.Components
{
    internal class TabSerializer : ITabSerializer
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            WriteIndented = true,
            Converters = { new PageContentInfoConverter(), new PointConverter() }
        };

        private bool _isModified;
        private readonly MultiPageTab _tab;

        public string FileExtension { get; } = "mpt";
        public bool IsModified => _isModified;

        public TabSerializer(MultiPageTab tab, PageActions pageActions)
        {
            _tab = tab;
            pageActions.CommandApplied += (_, _) => _isModified = true;
        }

        public void Load()
        {
            TabInfo tabInfo = JsonSerializer.Deserialize<TabInfo>(File.ReadAllText(_tab.SourcePath), _options);

            _tab.NotePages.Clear();
            foreach (var pageInfo in tabInfo.NotePages) 
            {
                NotePage page = new NotePage();

                foreach (var contentInfo in pageInfo.ContentInfos)
                {
                    page.Contents.PushFront(ContentRegistry.ResolveInfoByType(contentInfo.GetType()).Creator(contentInfo));
                }

                _tab.NotePages.Add(page);
            }

            _isModified = false;
        }

        public void Save()
        {
            NotePageInfo[] pageInfos = new NotePageInfo[_tab.NotePages.Count];
            for (int i = 0; i < pageInfos.Length; i++)
            {
                pageInfos[i] = new NotePageInfo
                {
                    ContentInfos = _tab.NotePages[i].Contents.DrawOrder.Select(content => content.GetSerializationInfo()).ToArray()
                };
            }

            TabInfo tabInfo = new TabInfo
            {
                NotePages = pageInfos
            };

            string json = JsonSerializer.Serialize(tabInfo, _options);
            File.WriteAllText(_tab.SourcePath, json);

            _isModified = false;
        }
    }
}
