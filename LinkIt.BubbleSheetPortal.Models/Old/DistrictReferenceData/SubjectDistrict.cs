using Envoc.Core.Shared.Model;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models.DistrictReferenceData
{
    public class SubjectDistrict : ValidatableEntity<SubjectDistrict>
    {
        private string name = string.Empty;
        private string shortName = string.Empty;
        private string gradeName = string.Empty;
        
        public int SubjectID { get; set; }
        public int GradeID { get; set; }
        public int StateID { get; set; }
        public int DistrictID { get; set; }
        
        public string Name
        {
            get { return name; }
            set { name = value.ConvertNullToEmptyString(); }
        }

        public string ShortName
        {
            get { return shortName; }
            set { shortName = value.ConvertNullToEmptyString(); }
        }

        public string GradeName
        {
            get { return gradeName; }
            set { gradeName = value.ConvertNullToEmptyString(); }
        }
    }
}