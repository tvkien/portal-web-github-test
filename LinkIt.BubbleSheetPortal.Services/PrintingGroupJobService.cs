using System.Linq;
using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Extensions;
using LinkIt.BubbleSheetPortal.Models;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class PrintingGroupJobService
    {
        private readonly IRepository<PrintingGroupJob> repository;

        public PrintingGroupJobService(IRepository<PrintingGroupJob> repository)
        {
            this.repository = repository;             
        }

        public bool DeletePrintingGroupById(int printingGroupJobId)
        {
            if (printingGroupJobId > 0)
            {
                PrintingGroupJob printingGroupjob = repository.Select().FirstOrDefault(o => o.PrintingGroupJobID == printingGroupJobId);
                if (printingGroupjob.IsNotNull())
                {
                    repository.Delete(printingGroupjob);
                    return true;
                }
            }
            return false;
        }

        public PrintingGroupJob Save(PrintingGroupJob printingGroupjob)
        {
            if (printingGroupjob.IsNotNull())
            {
                repository.Save(printingGroupjob);
                return printingGroupjob;
            }
            return null;
        }

        public PrintingGroupJob GetById(int printingGroupJobId)
        {
            return repository.Select().FirstOrDefault(o => o.PrintingGroupJobID == printingGroupJobId);
        }
    }
}