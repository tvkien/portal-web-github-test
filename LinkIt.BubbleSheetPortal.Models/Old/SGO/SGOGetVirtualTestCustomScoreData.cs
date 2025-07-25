using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOGetVirtualTestCustomScoreData
    {
        public int GradeId { get; set; }
        public string GradeName { get; set; }
        public int GradeOrder { get; set; }
        public int SubjectId { get; set; }
        public string SubjectName { get; set; }
        public int BankId { get; set; }
        public string BankName { get; set; }
        public int VirtualTestId { get; set; }
        public string VirtualTestName { get; set; }
        public int VirtualTestCustomScoreId { get; set; }
    }
}
