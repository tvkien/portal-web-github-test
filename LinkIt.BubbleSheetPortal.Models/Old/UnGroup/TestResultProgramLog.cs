using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TestResultProgramLog
    {
        public int TestResultProgramLogID { get; set; }
        public int TestResultProgramID { get; set; }
        public int TestResultID { get; set; }
        public int ProgramID { get; set; }
    }
}
