using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class SchoolTestResultDistrictRepository : IReadOnlyRepository<SchoolTestResultDistrict>
    {
        private readonly Table<SchoolTestResultDistrictView> _table;

        public SchoolTestResultDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<SchoolTestResultDistrictView>();
        }

        public IQueryable<SchoolTestResultDistrict> Select()
        {
            return _table.Select(x => new SchoolTestResultDistrict
                                    {
                                        SchoolId = x.SchoolID,
                                        Name = x.Name,
                                        DistrictId = x.DistrictID,
                                        VirtualTestId = x.VirtualTestID,
                                        ClassId = x.ClassID ?? 0,
                                        StudentId = x.StudentID,
                                        UserId = x.UserID ?? 0,
                                        VirtualTestSourceId = x.VirtualTestSourceID
                                    });
        }
    }
}
