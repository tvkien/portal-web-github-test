using LinkIt.BubbleSheetPortal.Models.TestMaker;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker
{
    public class RevertItemOutputDto
    {
        public int OldQTISchemaID { get; set; }
        public int NewQTISchemaID { get; set; }
        public IList<VirtualQuestionData> VirtualQuestions { get; set; }
    }
}
