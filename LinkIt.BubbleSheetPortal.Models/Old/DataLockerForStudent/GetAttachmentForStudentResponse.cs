using LinkIt.BubbleSheetPortal.Models.Old.UserGroup;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.Old.DataLockerForStudent
{
    public class GetAttachmentForStudentResponse
    {
        public int TotalRecord { get; set; }

        public IEnumerable<AttachmentForStudentModel> Data { get; set; } = Enumerable.Empty<AttachmentForStudentModel>();
    }
}
