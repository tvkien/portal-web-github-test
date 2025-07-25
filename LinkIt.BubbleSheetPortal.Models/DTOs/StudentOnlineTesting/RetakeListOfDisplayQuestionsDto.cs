using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.StudentOnlineTesting
{
    public class RetakeListOfDisplayQuestionsDto
    {
        public int StudentID { get; set; }

        public int? ClassID { get; set; }

        public string ListOfDisplayQuestions { get; set; }
    }
}
