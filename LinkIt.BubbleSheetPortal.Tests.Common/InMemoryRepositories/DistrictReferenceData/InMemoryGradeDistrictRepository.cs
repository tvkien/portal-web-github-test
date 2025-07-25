using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.DistrictReferenceData
{
    public class InMemoryGradeDistrictRepository:IReadOnlyRepository<GradeDistrict>
    {
        private readonly List<GradeDistrict> table;

        public InMemoryGradeDistrictRepository()
        {
            table = AddGradeDistrict();
        }

        private List<GradeDistrict> AddGradeDistrict()
        {
            return new List<GradeDistrict>()
            {
                new GradeDistrict{ DistrictID = 2, GradeID = 1, Name = "1", Order = 3 },
                new GradeDistrict{ DistrictID = 2, GradeID = 2, Name = "2", Order = 5 },
                new GradeDistrict{ DistrictID = 2, GradeID = 3, Name = "3", Order = 6 },
                new GradeDistrict{ DistrictID = 2, GradeID = 4, Name = "4", Order = 7 },
                new GradeDistrict{ DistrictID = 2, GradeID = 5, Name = "5", Order = 8 },
                new GradeDistrict{ DistrictID = 2, GradeID = 6, Name = "6", Order = 9 }
            };
        }

        public IQueryable<GradeDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
