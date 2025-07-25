using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentGrade
    {
        private string gradeName = string.Empty;
        public int StudentID { get; set; }
        public int GradeID { get; set; }
        
        public string GradeName
        {
            get { return gradeName; }
            set { gradeName = value.ConvertNullToEmptyString(); }
        }
        public int Order { get; set; }

    }
}