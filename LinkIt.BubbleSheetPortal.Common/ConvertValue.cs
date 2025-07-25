using System;
using System.ComponentModel;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class ConvertValue
    {
        public static int ToInt(string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return 0;
        }

        public static int? ToIntNullable(string value)
        {
            int result;
            if (int.TryParse(value, out result))
            {
                return result;
            }

            return null;
        }

        public static double ToJavaScriptMilliseconds(System.DateTime dt)
        {
            var result = dt.Subtract(new System.DateTime(1970, 1, 1, 0, 0, 0, System.DateTimeKind.Utc))
               .TotalMilliseconds;

            return result;
        }

        public static string ToZFormat(System.DateTime dt)
        {
            var result = dt.ToString("yyyy-MM-ddTHH\\:mm\\:ssZ");

            return result;
        }

        public static DateTime ConvertDateTimeByTimeZone(DateTime dateTime, int utcOffsetValue)
        {
            TimeZoneInfo timeZoneInfo = TimeZoneInfo.Local;
            TimeSpan baseUtcOffset = timeZoneInfo.BaseUtcOffset;
            var currentUtcOffset = baseUtcOffset.Hours;
            if (currentUtcOffset != utcOffsetValue)
            {
                dateTime = dateTime.AddHours(currentUtcOffset - utcOffsetValue);
            }
            return dateTime;
        }

        public static string GetAlphabets(int index)
        {
            if (index < 32)
            {
                int iAlphabet = 97 + index;
                return ((char)iAlphabet).ToString();
            }
            return index.ToString();            
        }
    }

    public static class Ext
    {
        public static TConvert ConvertTo<TConvert>(this object entity) where TConvert : new()
        {
            var convertProperties = TypeDescriptor.GetProperties(typeof(TConvert)).Cast<PropertyDescriptor>();
            var entityProperties = TypeDescriptor.GetProperties(entity).Cast<PropertyDescriptor>();

            var convert = new TConvert();

            foreach (var entityProperty in entityProperties)
            {
                var property = entityProperty;
                var convertProperty = convertProperties.FirstOrDefault(prop => prop.Name == property.Name);
                if (convertProperty != null)
                {
                    convertProperty.SetValue(convert, Convert.ChangeType(entityProperty.GetValue(entity), convertProperty.PropertyType));
                }
            }

            return convert;
        }
    }
}
