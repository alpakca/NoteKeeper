using System;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NoteKeeper.InspectionWindow.MultiPage.Serialization
{
    internal class PageContentInfoConverter : JsonConverter<PageContentInfo>
    {
        private const string TypeDiscriminator = "$type";
        private static readonly JsonSerializerOptions _options = new JsonSerializerOptions
        {
            Converters = { new PointConverter() }
        };

        public override PageContentInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonObject obj = JsonNode.Parse(ref reader).AsObject();

            if (!obj.TryGetPropertyValue(TypeDiscriminator, out JsonNode node))
            {
                throw new JsonException();
            }

            ContentRegistryEntry entry = ContentRegistry.ResolveInfo(node.GetValue<string>());

            return (PageContentInfo)obj.Deserialize(entry.InfoType, _options);
        }

        public override void Write(Utf8JsonWriter writer, PageContentInfo value, JsonSerializerOptions options)
        {
            ContentRegistryEntry entry = ContentRegistry.ResolveInfoByType(value.GetType());

            JsonNode node = JsonSerializer.SerializeToNode(value, entry.InfoType, _options);
            node[TypeDiscriminator] = JsonValue.Create(entry.TypeName);
            node.WriteTo(writer);
        }
    }
}
