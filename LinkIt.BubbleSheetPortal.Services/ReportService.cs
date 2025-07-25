using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Service;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class ReportService : PersistableModelService<Report>
    {
        public ReportService(IRepository<Report> repository) : base(repository)
        {
            this.repository = repository;
        }

        public IQueryable<Report> GetAllReports()
        {
            return repository.Select();
        }

        public Report GetReportById(int reportId)
        {
            return repository.Select().FirstOrDefault(x => x.Id.Equals(reportId));
        }
    }
}