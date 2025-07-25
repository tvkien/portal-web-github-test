using LinkIt.BubbleSheetPortal.VaultProvider.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.Models.TLDS
{
    public class TLDSDownloadInfo
    {
        public TLDSDownloadInfo()
        {
            Errors = new List<string>();
        }
        public int Total { get; set; }
        public int Completed { get; set; }
        public List<string> Errors { get; set; }
    }
}