using System.Collections.Generic;
using System.Linq;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.UserDefinedTypesDto
{
    public class IntegerListDto
    {
        public int Id { get; set; }
    }

    public static class IntegerListExtension
    {
        public static IEnumerable<IntegerListDto> ConvertToIntegerList(this IEnumerable<int> list)
        {
            if (list == null || !list.Any())
            {
                return Enumerable.Empty<IntegerListDto>();
            }

            return list.Where(x => x > 0)
                .Distinct()
                .Select(x => new IntegerListDto { Id = x })
                .ToList();
        }
    }
}
