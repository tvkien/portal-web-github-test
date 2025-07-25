using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Reflection;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils
{
    internal class DocumentTargetConverter : StringEnumConverter
    {
        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            if (!objectType.GetTypeInfo().IsAssignableFrom(typeof(DocumentTarget)))
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            if (reader.TokenType != JsonToken.String)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            var value = reader.Value.ToString();

            return System.Enum.TryParse<DocumentTarget>(value, true, out var target)
                ? target
                : DocumentTarget.None;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                writer.WriteNull();
                return;
            }

            writer.WriteValue(value.ToString().ToLowerInvariant());
        }
    }
}
