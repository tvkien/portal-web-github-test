using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.DistrictReferenceData
{
    public class InMemorySubjectDistrictRepository:IReadOnlyRepository<SubjectDistrict>
    {
        private readonly List<SubjectDistrict> table;

        public InMemorySubjectDistrictRepository()
        {
            table = AddSubjectDistricts();
        }

        private List<SubjectDistrict> AddSubjectDistricts()
        {
            return new List<SubjectDistrict>()
            {
                new SubjectDistrict{ SubjectID = 1, DistrictID = 1, GradeID = 1, GradeName = "Grade Name 1", Name = "Sub name 1", ShortName= "Short name 1", StateID = 1 },
                new SubjectDistrict{ SubjectID = 2, DistrictID = 1, GradeID = 1, GradeName = "Grade Name 2", Name = "Sub name 2", ShortName= "Short name 2", StateID = 1 },
                new SubjectDistrict{ SubjectID = 3, DistrictID = 1, GradeID = 1, GradeName = "Grade Name 3", Name = "Sub name 3", ShortName= "Short name 3", StateID = 1 },
                new SubjectDistrict{ SubjectID = 4, DistrictID = 1, GradeID = 1, GradeName = "Grade Name 4", Name = "Sub name 4", ShortName= "Short name 4", StateID = 1 },
                new SubjectDistrict{ SubjectID = 5, DistrictID = 1, GradeID = 1, GradeName = "Grade Name 5", Name = "Sub name 5", ShortName= "Short name 5", StateID = 1 }
            };
        }

        public IQueryable<SubjectDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
