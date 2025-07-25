using LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Enums;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils
{
    internal class RoleConverter : StringEnumConverter
    {
        private static readonly Hashtable Roles;
        static RoleConverter()
        {
            Roles = GetUris(typeof(RoleEnum));
        }

        public override object ReadJson(JsonReader reader, Type objectType, object existingValue, JsonSerializer serializer)
        {
            if (reader.TokenType == JsonToken.Null)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            if (!objectType.GetTypeInfo().IsAssignableFrom(typeof(RoleEnum)))
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            if (reader.TokenType != JsonToken.String)
            {
                return base.ReadJson(reader, objectType, existingValue, serializer);
            }

            var value = reader.Value.ToString();
            if (Roles.ContainsKey(value))
            {
                var role = (RoleEnum)Roles[value];
                return role;
            }

            return RoleEnum.Unknown;
        }

        private static Hashtable GetUris(Type type)
        {
            var roles = new Hashtable();
            foreach (System.Enum value in System.Enum.GetValues(type))
            {
                var uris = value.GetUris();
                if (uris != null && uris.Length > 0)
                {
                    foreach (var uri in uris)
                    {
                        roles.Add(uri, value);
                        // Only map the Enum back to the full URI
                        if (uri.StartsWith("http"))
                        {
                            roles.Add(value, uri);
                        }
                    }
                }
            }

            return roles;
        }

        public override void WriteJson(JsonWriter writer, object value, JsonSerializer serializer)
        {
            if (value == null)
            {
                base.WriteJson(writer, null, serializer);
                return;
            }

            if (Roles.ContainsKey(value))
            {
                var uri = Roles[value];
                writer.WriteValue(uri);
                return;
            }

            base.WriteJson(writer, value, serializer);
        }
    }
}
