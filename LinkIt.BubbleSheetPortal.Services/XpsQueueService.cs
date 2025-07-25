using Envoc.Core.Shared.Data;
using Envoc.Core.Shared.Service;
using LinkIt.BubbleSheetPortal.Models.Old.XpsQueue;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Services
{
    public class XpsQueueService : PersistableModelService<XpsQueue>
    {
        public XpsQueueService(IRepository<XpsQueue> repository) : base(repository) 
        {
        }
    }
}
