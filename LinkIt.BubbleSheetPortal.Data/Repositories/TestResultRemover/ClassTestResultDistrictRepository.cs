using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class ClassTestResultDistrictRepository : IReadOnlyRepository<ClassTestResultDistrict>
    {
        private readonly Table<ClassTestResultDistrictView> _table;

        public ClassTestResultDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<ClassTestResultDistrictView>();
        }

        public IQueryable<ClassTestResultDistrict> Select()
        {
            return _table.Select(x => new ClassTestResultDistrict
                                    {
                                        ClassId = x.ClassID,
                                        Name = x.ClassCustom,
                                        DistrictId = x.DistrictID,
                                        VirtualTestId = x.VirtualTestID,
                                        StudentId = x.StudentID,
                                        SchoolId = x.SchoolID ?? 0,
                                        UserId = x.UserID ?? 0 ,
                                        UserName = x.UserName ,
                                        VirtualTestSourceId = x.VirtualTestSourceID
                                    });
        }
    }
}
