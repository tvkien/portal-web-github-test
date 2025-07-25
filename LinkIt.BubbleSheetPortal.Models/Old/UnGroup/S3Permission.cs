using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class S3Permission
    {
        public int Id { get; set; }
        public int S3LinkitPortalId { get; set; }
        public int UserId { get; set; }
        public DateTime CreateDate { get; set; }
    }
}
