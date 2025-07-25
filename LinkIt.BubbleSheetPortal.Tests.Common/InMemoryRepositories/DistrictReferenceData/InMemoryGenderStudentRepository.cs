using System.Collections.Generic;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.DistrictReferenceData
{
    public class InMemoryGenderStudentRepository: IReadOnlyRepository<GenderStudent>
    {
        private readonly List<GenderStudent> table;

        public InMemoryGenderStudentRepository()
        {
            table = AddGenderStudents();
        }

        private List<GenderStudent> AddGenderStudents()
        {
            return new List<GenderStudent>()
            {
                new GenderStudent{ Code = 'F', DistrictID = 1, Name = "Female" },
                new GenderStudent{ Code = 'M', DistrictID = 1, Name = "Male" },
                new GenderStudent{ Code = 'U', DistrictID = 1, Name = "Unknown" }
            };
        }

        public IQueryable<GenderStudent> Select()
        {
            return table.AsQueryable();
        }
    }
}
