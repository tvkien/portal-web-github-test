using System;
using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.UserDefinedTypesDto
{
    public class StringList
    {
        public string Value { get; set; }
    }

    public static class StringListExtension
    {
        public static IEnumerable<StringList> ConvertToStringList(this IEnumerable<string> list)
        {
            if (list == null || !list.Any())
            {
                return Enumerable.Empty<StringList>();
            }

            return list.Where(x => !string.IsNullOrEmpty(x))
                .Select(x => x.Trim())
                .Where(x => !string.IsNullOrEmpty(x))
                .Select(x => new StringList { Value = x })
                .ToList();
        }
    }
}
