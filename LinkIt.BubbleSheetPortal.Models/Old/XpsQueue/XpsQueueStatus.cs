using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.XpsQueue
{
    public enum XpsQueueStatus
    {
        Pending = 2,
        Started = 3,
        Cancelled = 4,
        OutputGenerated = 5,
        Complete = 6,
        UploadingData = 8
    }
}
