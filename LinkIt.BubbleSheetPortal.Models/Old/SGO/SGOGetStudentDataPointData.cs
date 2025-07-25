using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOGetStudentDataPointData
    {
        public int SgoId { get; set; }
        public int SgoStudentId { get; set; }
        public int StudentId { get; set; }
        public int ClassId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }        
        public string StudentName { get; set; }
        public string Code { get; set; }
        public int? TestResultId { get; set; }
        public decimal? ScoreRaw { get; set; }
        public DateTime? ResultDate { get; set; }
        public decimal? PointsPossible { get; set; }
        public int? SGOStudentType { get; set; }
    }
}
