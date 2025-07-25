using System.Collections.Generic;
using System.Linq;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using Envoc.Core.Shared.Data;

namespace LinkIt.BubbleSheetPortal.Tests.Common.InMemoryRepositories.DistrictReferenceData
{
    public class InMemoryClusterDistrictRepository:IReadOnlyRepository<ClusterDistrict>
    {
        private readonly List<ClusterDistrict> table;

        public InMemoryClusterDistrictRepository()
        {
            table = AddClusterDistricts();
        }

        private List<ClusterDistrict> AddClusterDistricts()
        {
            return new List<ClusterDistrict>()
            {
                new ClusterDistrict{ DistrictID = 1, ListClusterName= "Writing", SubjectName = "Language Arts" },
                new ClusterDistrict{ DistrictID = 1, ListClusterName= "First Writing", SubjectName = "Language Arts" },
                new ClusterDistrict{ DistrictID = 1, ListClusterName= "Second Writing", SubjectName = "Language Arts" },
                new ClusterDistrict{ DistrictID = 2, ListClusterName= "Reading", SubjectName = "Language Arts" },
                new ClusterDistrict{ DistrictID = 3, ListClusterName= "Work w/Interpret", SubjectName = "Language Arts" },
                new ClusterDistrict{ DistrictID = 1, ListClusterName= "Analyzing / Critiquing Text", SubjectName = "Language Arts" },
                new ClusterDistrict{ DistrictID = 1, ListClusterName= "Reading", SubjectName = "Language Arts" }
            };
        }

        public IQueryable<ClusterDistrict> Select()
        {
            return table.AsQueryable();
        }
    }
}
