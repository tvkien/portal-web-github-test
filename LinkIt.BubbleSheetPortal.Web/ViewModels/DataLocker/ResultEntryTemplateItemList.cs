using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.DataLocker
{
    public class ResultEntryTemplateItemList
    {
        public int VirtualTestCustomScoreID { get; set; }
        public string Name { get; set; }

        public string Author { get; set; }

        public DateTime? CreatedDate { get; set; }

        public DateTime? UpdatedDate
        {
             get; set; 
        }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int TotalVirtualTestAssociated { get; set; }

        public string PublishedDistricts { get; set; }

        public bool IsPublished { get; set; }
        public bool Archived { get; set; }
    }
}
