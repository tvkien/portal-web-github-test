using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class APIAccount
    {
        public int APIAccountID { get; set; }
        public Guid ClientAccessKeyID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int Status { get; set; }
        public int Type { get; set; }
        public string LinkitPublicKey { get; set; }
        public string LinkitPrivateKey { get; set; }
        public int APIAccountTypeID { get; set; }
        public int TargetID { get; set; }
    }
}
