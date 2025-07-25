using LinkIt.BubbleSheetPortal.Common;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.LTI.Utils
{
    public static class EnumExtensions
    {
        /// <summary>
        /// Return the URI associated with this Enum value.
        /// </summary>
        public static string[] GetUris(this System.Enum value)
        {
            return
                value.GetType()
                .GetRuntimeFields()
                .SingleOrDefault(f => f.Name == value.ToString())
                ?.GetCustomAttributes<UriAttribute>()
                .Select(a => a.Uri)
                .ToArray();
        }
        public static string GetDisplayName(this System.Enum enumValue)
        {
            return enumValue.GetType()
                            .GetMember(enumValue.ToString())
                            .First()
                            .GetCustomAttribute<DisplayAttribute>()
                            .GetName();
        }
    }
}
