using NoteKeeper.InspectionWindow.PagedTabs.Serialization;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;

namespace NoteKeeper.InspectionWindow.PagedTabs.MultiPage
{
    internal class TabSerializer : ITabSerializer
    {
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Converters = { new PageContentInfoConverter() }
        };

        private readonly MultiPageTab _tab;

        public string FileExtension { get; } = "mpt";

        public TabSerializer(MultiPageTab tab)
        {
            _tab = tab;
        }

        public void Load(string filePath)
        {
            NotePageInfo[] pageInfos = JsonSerializer.Deserialize<NotePageInfo[]>(File.ReadAllText(filePath), _options);

            _tab.NotePages.Clear();
            foreach (var pageInfo in pageInfos) 
            {
                NotePage page = new NotePage();

                foreach (var contentInfo in pageInfo.ContentInfos)
                {
                    page.Contents.Add(ContentRegistry.ResolveInfo(contentInfo.TypeName).Creator(contentInfo));
                }

                _tab.NotePages.Add(page);
            }
        }

        public void Save(string filePath)
        {
            NotePageInfo[] pageInfos = new NotePageInfo[_tab.NotePages.Count];
            for (int i = 0; i < pageInfos.Length; i++)
            {
                pageInfos[i] = new NotePageInfo
                {
                    ContentInfos = _tab.NotePages[i].Contents.Select(content => content.GetSerializationInfo()).ToArray()
                };
            }

            string json = JsonSerializer.Serialize(pageInfos, _options);
            File.WriteAllText(filePath, json);
        }
    }
}
