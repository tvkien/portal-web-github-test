using LinkIt.BubbleSheetPortal.Models.Enums;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement
{
    public class AliveConfirmDto
    {
        public int UserId { get; set; }

        public Guid documentGuid { get; set; }

        public int DocumentRawId { get; set; }

        public DocumentTypeLock DocumentTypeLock
        {
            get
            {
                return documentGuid != Guid.Empty ? DocumentTypeLock.EDMDocument : DocumentTypeLock.IMArtifact;
            }
        }
    }
}
