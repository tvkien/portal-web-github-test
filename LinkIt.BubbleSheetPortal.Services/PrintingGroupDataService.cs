using System.Linq;
using Envoc.Core.Shared.Data;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class PrintingGroupDataService
    {
        private readonly IReadOnlyRepository<PrintingGroupData> repository;

        public PrintingGroupDataService(IReadOnlyRepository<PrintingGroupData> repository)
        {
            this.repository = repository;
        }

        public IQueryable<PrintingGroupData> GetPrintingGroupDataByGroupID(int groupId)
        {
            return repository.Select().Where(o => o.GroupID==groupId);
        }
    }
}