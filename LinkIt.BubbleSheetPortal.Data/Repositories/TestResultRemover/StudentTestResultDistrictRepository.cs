using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class StudentTestResultDistrictRepository : IReadOnlyRepository<StudentTestResultDistrict>
    {
        private readonly Table<StudentTestResultDistrictView> _table;

        public StudentTestResultDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<StudentTestResultDistrictView>();
        }

        public IQueryable<StudentTestResultDistrict> Select()
        {
            return _table.Select(x => new StudentTestResultDistrict
                                    {
                                        StudentId = x.StudentID,
                                        DistrictId = x.DistrictID,
                                        StudentCustom = x.StudentCustom,
                                        ClassId = x.ClassID ?? 0,
                                        VirtualTestId = x.VirtualTestID,
                                        SchoolId = x.SchoolID ?? 0,
                                        UserId = x.UserID ?? 0,
                                        VirtualTestSourceId = x.VirtualTestSourceID
                                    });
        }
    }
}
