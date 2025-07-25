using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class DisplayTestResultDistrictRepository : IReadOnlyRepository<DisplayTestResultDistrict>
    {
        private readonly Table<DisplayTestResultDistrictView> _table;

        public DisplayTestResultDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<DisplayTestResultDistrictView>();
        }

        public IQueryable<DisplayTestResultDistrict> Select()
        {
            return _table.Select(x => new DisplayTestResultDistrict
                                    {
                                        TestResultId = x.TestResultID,
                                        VirtualTestId = x.VirtualTestID,
                                        TestName = x.TestNameCustom,
                                        AuthorUserId = x.AuthorUserID ?? 0,
                                        SchoolId = x.SchoolID ?? 0,
                                        SchoolName = x.SchoolName,
                                        UserId = x.UserID,
                                        TeacherCustom = x.TeacherCustom,
                                        ClassId = x.ClassID ?? 0,
                                        ClassNameCustom = x.ClassNameCustom,
                                        StudentId = x.StudentID, 
                                        DistrictId = x.DistrictID,
                                        StudentCustom =x.StudentCustom
                                    });
        }
    }
}
