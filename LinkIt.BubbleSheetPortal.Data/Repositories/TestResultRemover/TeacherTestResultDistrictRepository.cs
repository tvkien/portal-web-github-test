using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class TeacherTestResultDistrictRepository : IReadOnlyRepository<TeacherTestResultDistrict>
    {
        private readonly Table<TeacherTestResultDistrictView> _table;

        public TeacherTestResultDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<TeacherTestResultDistrictView>();
        }

        public IQueryable<TeacherTestResultDistrict> Select()
        {
            return _table.Select(x => new TeacherTestResultDistrict
                                    {
                                        DistrictId = x.DistrictID,
                                        UserName = x.UserName ,
                                        UserId = x.UserID,
                                        VirtualTestId = x.VirtualTestID,           
                                        ClassId = x.ClassID ?? 0,
                                        StudentId = x.StudentID ,
                                        SchoolId = x.SchoolID ?? 0 ,
                                        VirtualTestSourceId = x.VirtualTestSourceID
                                    });
        }
    }
}
