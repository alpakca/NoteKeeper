using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NoteKeeper.InspectionWindow.PagedTabs.Serialization
{
    internal class PageContentInfoConverter : JsonConverter<PageContentInfo>
    {
        private const string TypeDiscriminator = "$type";

        public override PageContentInfo Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonObject obj = JsonNode.Parse(ref reader).AsObject();
            
            if (!obj.TryGetPropertyValue(TypeDiscriminator, out JsonNode node))
            {
                throw new JsonException();
            }

            // var entry = _registry.GetEntry(node.GetValue<string>());

            return obj.Deserialize<PageContentInfo>();
        }

        public override void Write(Utf8JsonWriter writer, PageContentInfo value, JsonSerializerOptions options)
        {
            string typeID = ContentRegistry.ResolveTypeID(value.GetType());
            ContentRegistryEntry entry = ContentRegistry.ResolveInfo(typeID);

            JsonNode node = JsonSerializer.SerializeToNode(value);
            node[TypeDiscriminator] = JsonValue.Create(entry.TypeName);
            node.WriteTo(writer);
        }
    }
}
