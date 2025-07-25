using System;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;

namespace LinkIt.BubbleSheetPortal.Web.Helpers
{
    public class DateTimeConverter : IsoDateTimeConverter
    {
        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value is DateTime && ((DateTime)value).Kind == DateTimeKind.Unspecified)
            {
                value = DateTime.SpecifyKind(((DateTime)value), DateTimeKind.Utc);
            }
            base.WriteJson(writer, value, serializer);
        }
    }
}