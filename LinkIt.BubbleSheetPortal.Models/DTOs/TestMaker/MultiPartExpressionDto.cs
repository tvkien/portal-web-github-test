using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.TestMaker
{
    public class MultiPartExpressionDto
    {
        public int MultiPartQTIItemExpressionId { get; set; }

        public int MultiPartVirtualQuestionExpressionId { get; set; }

        public string Expression { get; set; }

        public string EnableElements { get; set; }

        public string Rules  { get; set; }

        public int Order { get; set; }
    }
}
