using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.DocumentManagement
{
    public class CancelUploadDto
    {
        public Guid DocumentGuid { get; set; }
        public string UploadId { get; set; }
    }
}
