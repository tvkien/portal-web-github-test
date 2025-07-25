using System.Linq;
using LinkIt.BubbleSheetPortal.Models.DistrictReferenceData;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using System.Data.Linq;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.DistrictReferenceData
{
    public class GenderStudentRepository : IReadOnlyRepository<GenderStudent>
    {
        private readonly Table<GenderStudentView> table;

        public GenderStudentRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            table = StudentDataContext.Get(connectionString).GetTable<GenderStudentView>();
        }

        public IQueryable<GenderStudent> Select()
        {
            return table.Select(x => new GenderStudent
            {
                Name = x.Name,
                Code = x.Code,
                DistrictID = x.DistrictID
            });
        }
    }
}