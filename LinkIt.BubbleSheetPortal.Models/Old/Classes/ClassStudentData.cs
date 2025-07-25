using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class ClassStudentData : ValidatableEntity<ClassStudentData>
    {
        private string code = string.Empty;

        public int ClassStudentID { get; set; }
        public int ClassID { get; set; }
        public int StudentID { get; set; }
        public bool? Active { get; set; }
        public int? SISID { get; set; }
        public int? DistrictID { get; set; }

        public string Code
        {
            get { return code; }
            set { code = value.ConvertNullToEmptyString(); }
        }
    }
}