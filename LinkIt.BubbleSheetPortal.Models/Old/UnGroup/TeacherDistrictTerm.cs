using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TeacherDistrictTerm
    {
        public int DistrictID { get; set; }
        private string districtName = string.Empty;
        
        public int UserId { get; set; }
        public int DistrictTermId { get; set; }
        public int SchoolId { get; set; }

        public string DistrictName
        {
            get { return districtName; }
            set { districtName = value.ConvertNullToEmptyString(); }
        }
        public int UserStatusId { get; set; }
    }
}