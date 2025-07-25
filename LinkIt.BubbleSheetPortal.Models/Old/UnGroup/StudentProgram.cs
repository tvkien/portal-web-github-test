using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentProgram : ValidatableEntity<StudentProgram>
    {
        private string programName = string.Empty;

        public int StudentProgramID { get; set; }
        public int StudentID { get; set; }
        public int ProgramID { get; set; }
        
        public string ProgramName
        {
            get { return programName; }
            set { programName = value.ConvertNullToEmptyString(); }
        }
        public int? AccessLevelId { get; set; }
    }
}