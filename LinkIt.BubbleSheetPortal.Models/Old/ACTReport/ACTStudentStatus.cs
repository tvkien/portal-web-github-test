using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ACTStudentStatus
    {
        public int StudentId { get; set; }

        public int VirtualTestId { get; set; }

        public int TestResultId { get; set; }

        public int VirtualQuestionId { get; set; }

        public int PointsEarned { get; set; }
    }
}
