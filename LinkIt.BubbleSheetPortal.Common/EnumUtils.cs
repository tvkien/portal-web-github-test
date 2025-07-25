using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class EnumUtils
    {
        public static string GetDescription<T>(T enumValue, string defDesc)
        {
            var fi = enumValue.GetType().GetField(enumValue.ToString());

            if (null != fi)
            {
                object[] attrs = fi.GetCustomAttributes
                        (typeof(DescriptionAttribute), true);
                if (attrs.Length > 0)
                    return ((DescriptionAttribute)attrs[0]).Description;
            }

            return defDesc;
        }
        public static string GetDescription<T>(T enumValue)
        {
            return GetDescription(enumValue, string.Empty);
        }

        public static T FromDescription<T>(string description)
        {
            var t = typeof(T);
            foreach (var fi in t.GetFields())
            {
                var attrs = fi.GetCustomAttributes
                        (typeof(DescriptionAttribute), true);
                if (attrs.Length > 0)
                {
                    foreach (DescriptionAttribute attr in attrs)
                    {
                        if (attr.Description.Equals(description))
                            return (T)fi.GetValue(null);
                    }
                }
            }
            return default(T);
        }

        public static IEnumerable<string> GetDescriptions(Type enumType)
        {
            var fields = enumType.GetFields();
            foreach (var field in fields)
            {
                var descriptionAttributes = field.GetCustomAttributes(false).OfType<DescriptionAttribute>();
                foreach (var descAttr in descriptionAttributes)
                {
                    yield return descAttr.Description;
                }
            }
        }
    }
}
