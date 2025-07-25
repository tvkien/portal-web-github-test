using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class VirtualTestTestResultDistrictRepository : IReadOnlyRepository<VirtualTestTestResultDistrict>
    {
        private readonly Table<VirtualTestTestResultDistrictView> _table;

        public VirtualTestTestResultDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<VirtualTestTestResultDistrictView>();
        }

        public IQueryable<VirtualTestTestResultDistrict> Select()
        {
            return _table.Select(x => new VirtualTestTestResultDistrict
                                    {
                                        VirtualTestId = x.VirtualTestID,
                                        Name = x.TestNameCustom,
                                        StudentId = x.StudentID,
                                        ClassId = x.ClassID ?? 0,
                                        DistrictId = x.DistrictID  ,
                                        SchoolId = x.SchoolID ?? 0,
                                        UserId = x.UserID ?? 0  ,
                                        VirtualTestSourceId = x.VirtualTestSourceID
                                    });
        }
    }
}
