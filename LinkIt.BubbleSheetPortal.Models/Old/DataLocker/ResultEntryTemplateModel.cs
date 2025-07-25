using System;
using System.Collections.Generic;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Models.DataLocker
{
    public class ResultEntryTemplateModel
    {
        public int VirtualTestCustomScoreID { get; set; }
        public string Name { get; set; }

        public string Author
        {
            get
            {
                if (!string.IsNullOrEmpty(NameFirst) && !string.IsNullOrEmpty(NameLast))
                {
                    return string.Format("{0}, {1}", NameLast, NameFirst);
                }
                else
                {
                    return string.Format("{0}{1}", NameLast, NameFirst);//NameFirst or NameLast
                }
            } 
        }

        public string CreatedDateDisplay
        {
            get { return CreatedDate.DisplayDateWithFormat(); }
        }

        public string UpdatedDateDisplay
        {
            get { return UpdatedDate.DisplayDateWithFormat(); }
        }

        public DateTime? CreatedDate { get; set; }
        public DateTime? UpdatedDate { get; set; }
        public string NameFirst { get; set; }
        public string NameLast { get; set; }

        public int? TotalVirtualTestAssociated { get; set; }

        public List<string> PublishedDistricts { get; set; }

        public bool IsPublished { get; set; }

        public bool IsMultiDate { get; set; }

        public bool Archived { get; set; }
    }
}
