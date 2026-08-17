using NoteKeeper.InspectionWindow.MultiPage.Serialization;
using System.IO;
using System.Linq;
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

        private readonly MultiPageTab _tab;

        public TabSerializer(MultiPageTab tab)
        {
            _tab = tab;
        }

        public TabSerializationResult Load()
        {
            string json;
            try
            {
                json = File.ReadAllText(_tab.SourcePath);
            }
            catch (FileNotFoundException)
            {
                return TabSerializationResult.FileNotFound;
            }
            catch (IOException)
            {
                return TabSerializationResult.IOError;
            }

            TabInfo tabInfo;
            try
            {
                tabInfo = JsonSerializer.Deserialize<TabInfo>(json, _options);
            }
            catch (JsonException)
            {
                return TabSerializationResult.WrongFileFormat;
            }

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

            _tab.TabHistory.MarkLoaded();

            return TabSerializationResult.Success;
        }

        public TabSerializationResult Save()
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
            try
            {
                File.WriteAllText(_tab.SourcePath, json);
            }
            catch (IOException)
            {
                return TabSerializationResult.IOError;
            }

            _tab.TabHistory.MarkSaved();
            return TabSerializationResult.Success;
        }
    }
}
