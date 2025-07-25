using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class ClassDistrictRepository : IReadOnlyRepository<ClassDistrict>
    {
        private readonly Table<ClassDistrictView> _table;

        public ClassDistrictRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<ClassDistrictView>();
        }

        public IQueryable<ClassDistrict> Select()
        {
            return _table.Select(x => new ClassDistrict
                                    {
                                        ClassId = x.ClassID,
                                        DistrictId = x.DistrictID,
                                        Name = x.ClassNameCustom ,
                                        VirtualTestSourceId = x.VirtualTestSourceID
                                    });
        } 
    }
}
