using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class QtiGroup
    {
        public int QtiGroupId { get; set; }
        public string Name { get; set; }
        public string Source { get; set; }
        public string Type { get; set; }
        public int UserId { get; set; }
        public int? GroupId { get; set; }
        public int? VirtualTestId { get; set; }
        public int? QtiBankId { get; set; }
        public string OldMasterCode { get; set; }
        public int AccessId { get; set; }
        public int OwnershipType { get; set; }
        public int? AuthorGroupId { get; set; }
        public int? SourceId { get; set; }
        public string AuthorGroupName { get; set; }
        public DateTime? CreatedDate { get; set; }
        public DateTime? ModifiedDate { get; set; }
        public string AuthorFirstName { get; set; }
        public string AuthorLastName { get; set; }
    }
}
