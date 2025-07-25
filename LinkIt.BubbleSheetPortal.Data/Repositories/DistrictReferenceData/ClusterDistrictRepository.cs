using System.Linq;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.DistrictReferenceData
{
    public class ClusterDistrictRepository : IReadOnlyRepository<ClusterDistrict>
    {
        private readonly Table<ClusterDistrictEntity> table;

        public ClusterDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = DbDataContext.Get(connectionString).GetTable<ClusterDistrictEntity>();
        }

        public IQueryable<ClusterDistrict> Select()
        {
            return table.Select(x => new ClusterDistrict
            {
                ListClusterName = x.ListClusterName,
                SubjectName = x.SubjectName,
                DistrictID = x.DistrictID
            });
        }
    }
}