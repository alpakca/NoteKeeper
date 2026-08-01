using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Nodes;
using System.Text.Json.Serialization;

namespace NoteKeeper.InspectionWindow.PagedTabs.Serialization
{
    internal class PointConverter : JsonConverter<Point>
    {
        public override Point Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
        {
            JsonObject obj = JsonNode.Parse(ref reader).AsObject();
            int x = obj["X"]?.GetValue<int>() ?? 0;
            int y = obj["Y"]?.GetValue<int>() ?? 0;

            return new Point(x, y);
        }

        public override void Write(Utf8JsonWriter writer, Point value, JsonSerializerOptions options)
        {
            writer.WriteStartObject();

            writer.WriteNumber("X", value.X);
            writer.WriteNumber("Y", value.Y);

            writer.WriteEndObject();
        }
    }
}
