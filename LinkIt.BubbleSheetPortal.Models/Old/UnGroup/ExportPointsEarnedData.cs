using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ExportPointsEarnedData
    {
        public int TestResultID { get; set; }

        public string ClassName;

        public int? UserID { get; set; }

        public string UserName { get; set; }

        public string Term { get; set; }

        public int? SchoolID { get; set; }

        public string SchoolName { get; set; }

        public int StudentID { get; set; }

        public string TestName { get; set; }

        public int QuestionOrder { get; set; }

        public int PointsEarned { get; set; }

        public int PointsPossible { get; set; }

        public bool WasAnswered { get; set; }
    }
}
