using Envoc.Core.Shared.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Data.Repositories.ClassPrinting
{
    public interface IClassPrintingGroupRepository : IRepository<Models.ClassPrintingGroup>
    {
        int CountActiveStudentInGroup(int groupId);
    }
}
