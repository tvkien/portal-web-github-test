using System.Data.Linq;
using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Data.Entities;
using LinkIt.BubbleSheetPortal.Models.TestResultRemover;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.TestResultRemover
{
    public class VirtualTestWithOutTestResultRepository : IReadOnlyRepository<VirtualTestWithOutTestResult>
    {
        private readonly Table<VirtualTestWithOutTestResultView> _table;

        public VirtualTestWithOutTestResultRepository(IConnectionString conn)
        {
            var connectionString = conn.GetLinkItConnectionString();
            _table = TestDataContext.Get(connectionString).GetTable<VirtualTestWithOutTestResultView>();
        }

        public IQueryable<VirtualTestWithOutTestResult> Select()
        {
            return _table.Select(x => new VirtualTestWithOutTestResult
                                    {
                                        VirtualTestId = x.VirtualTestID,
                                        AuthorUserId = x.AuthorUserID ?? 0,
                                        Name = x.Name ,
                                        DistrictId = x.DistrictID,
                                        ParentTestID = x.ParentTestID,
                                        OriginalTestID = x.OriginalTestID
                                    });
        }
    }
}
