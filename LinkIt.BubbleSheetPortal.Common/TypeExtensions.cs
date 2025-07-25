using LinkIt.BubbleSheetPortal.Common.DataFileUpload.DataFileProcessingCertica;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Common
{
    public static class TypeExtensions
    {
        public static IList<String> GetDisplayNames(this Type type)
        {
            var props = TypeDescriptor.GetProperties(type);
            var propDisplayNames = new List<String>();

            foreach (PropertyDescriptor prop in props)
            {
                propDisplayNames.Add(prop.DisplayName);
            }

            return propDisplayNames;
        }

        public static string GetDisplayName(this Type type, String propName)
        {
            var props = TypeDescriptor.GetProperties(type);
            return props[propName].DisplayName;
        }
        public static bool In<T>(this T theObject, params T[] theComparisionArray)
        {
            return (theComparisionArray ?? new T[0]).Contains(theObject);
        }
        public static IEnumerable<string> GetAllPropertiesName(this Type type)
        {
            return type.GetProperties()
                .Select(c => c.Name);
        }
    }
}
