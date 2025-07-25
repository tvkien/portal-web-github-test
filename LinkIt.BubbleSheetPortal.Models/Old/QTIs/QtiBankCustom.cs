using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiBankCustom
    {
        public int QTIBankId { get; set; }
        public string Name { get; set; }
        public string AuthorGroupName { get; set; }
        public int? AuthorGroupId { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
        public DateTime? ModifiedDate { get; set; }
    }
}
