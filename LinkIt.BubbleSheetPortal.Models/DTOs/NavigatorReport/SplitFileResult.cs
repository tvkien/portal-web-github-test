using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.NavigatorReport
{
    public class SplitFileResult
    {
        public byte[] FileBinary { get; set; }
        public NavigatorMetaData Metadata { get; set; }
    }
}
