using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class MasterStandardResource
    {
        public int MasterStandardID { get; set; }
        public string State { get; set; }
        public string Document { get; set; }
        public string Subject { get; set; }
        public int Year { get; set; }
        public string Grade { get; set; }
        public int Level { get; set; }
        public int Children { get; set; }
        public string Label { get; set; }
        public string Number { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string GUID { get; set; }
        public string ParentGUID { get; set; }
        public string LoGrade { get; set; }
        public string HiGrade { get; set; }
        public int? LowGradeID { get; set; }
        public int? HighGradeID { get; set; }
        public int? StateId { get; set; }
        public bool Archived { get; set; }
        public int CountChildren { get; set; }
    }
}
