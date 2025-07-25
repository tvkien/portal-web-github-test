using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using LinkIt.BubbleSheetPortal.Common;
namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    //The index of each field is mapped to DataTable
    public class SGOAuditTrailSearchItem
    {
        //0
        public int? ID { get; set; }

        //1
        public string SourceOfData { get; set; }

        //2
        public DateTime? CreatedDate { get; set; }

        //3
        public int? UserID { get; set; }

        //4
        public string FirstName { get; set; }

        //5
        public string LastName { get; set; }

        //6
        public string Details { get; set; }

        //7
        public int? ActionType { get; set; }

        //8
        public string ReferenceData { get; set; }

        //9
        public string CreatedDateStr
        {
            get
            {
                if (CreatedDate == null) return string.Empty;
                //return String.Format("{0:MMM dd, yyyy HH:mm}", CreatedDate);
                return CreatedDate.Value.DisplayDateWithFormat();
            }
        }

        public List<int> StudentIDs { get; set; } 
        public List<int> GroupIDs { get; set; } 
        public List<int> SGODataPointIDs { get; set; } 
    }
}
