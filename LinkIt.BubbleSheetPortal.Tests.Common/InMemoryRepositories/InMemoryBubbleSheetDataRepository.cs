using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories
{
    public class InMemoryBubbleSheetDataRepository : IReadOnlyRepository<BubbleSheetData>
    {
        private readonly List<BubbleSheetData> table = new List<BubbleSheetData>();

        public InMemoryBubbleSheetDataRepository()
        {
            table = AddBubbleSheetData();
        }

        private static List<BubbleSheetData> AddBubbleSheetData()
        {
            return new List<BubbleSheetData>
                       {
                           new BubbleSheetData{ TestId = 1, BankId = 1, StateId = 1, GradeId = 1, SubjectId = 1, DistrictId = 1, ClassId = 1, SchoolId = 1, UserId = 1, BubbleSizeId = 25, DistrictTermId = 1, CreatedByUserId = 699, StudentIdList = new List<string> { "10", "11", "13", "14", "15" }},
                           new BubbleSheetData{ TestId = 2, BankId = 2, StateId = 2, GradeId = 2, SubjectId = 2, DistrictId = 2, ClassId = 2, SchoolId = 2, UserId = 2},
                           new BubbleSheetData{ TestId = 1, BankId = 1, StateId = 5, GradeId = 1, SubjectId = 1, DistrictId = 4, ClassId = 1, SchoolId = 1, UserId = 1, DistrictTermId = 1 },
                           new BubbleSheetData{ TestId = 1, BankId = 1, StateId = 4, GradeId = 1, SubjectId = 1, DistrictId = 1, ClassId = 1, SchoolId = 1, UserId = 1, DistrictTermId = 1 }
                       };
        }

        public IQueryable<BubbleSheetData> Select()
        {
            return table.AsQueryable();
        }
    }
}
