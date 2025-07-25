using System.Linq;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.DistrictReferenceData
{
    public class SubjectDistrictRepository : IReadOnlyRepository<SubjectDistrict>
    {
        private readonly Table<SubjectDistrictView> table;

        public SubjectDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = TestDataContext.Get(connectionString).GetTable<SubjectDistrictView>();
        }

        public IQueryable<SubjectDistrict> Select()
        {
            return table.Select(x => new SubjectDistrict
            {
                SubjectID = x.SubjectID,
                GradeID = x.GradeID,
                Name = x.Name,
                StateID = x.StateID,
                ShortName = x.ShortName,
                DistrictID = x.DistrictID,
                GradeName = x.GradeName
            });
        }
    }
}