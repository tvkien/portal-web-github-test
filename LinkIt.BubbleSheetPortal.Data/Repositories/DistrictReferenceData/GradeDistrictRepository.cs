using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.DistrictReferenceData
{
    public class GradeDistrictRepository : IReadOnlyRepository<GradeDistrict>
    {
        private readonly Table<GradeDistrictView> table;

        public GradeDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<GradeDistrictView>();
        }

        public IQueryable<GradeDistrict> Select()
        {
            return table.Select(x => new GradeDistrict
            {
                GradeID = x.GradeID,
                Name = x.Name,
                Order = x.Order,
                DistrictID = x.DistrictID
            });
        }
    }
}