using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class TeacherTestDistrictTerm
    {
        public int DistrictTermId { get; set; }
        public string DistrictTermName { get; set; }
        public DateTime? DateStart { get; set; }
        public DateTime? DateEnd { get; set; }
        public int VirtualTestId { get; set; }
        public int? VirtualTestSubTypeId { get; set; }
        public int UserId { get; set; }
        public string UserName { get; set; }
        public string NameLast { get; set; }
        public string NameFirst { get; set; }
        public int ClassId { get; set; }
        public string ClassName { get; set; }
        public int? SchoolId { get; set; }
        public string SchoolName { get; set; }
        public int DistrictId { get; set; }
        public DateTime ResultDate { get; set; }

        public override bool Equals(object obj)
        {
            var t = obj as TeacherTestDistrictTerm;
            return t != null && DistrictTermId.Equals(t.DistrictTermId) && VirtualTestId.Equals(t.VirtualTestId) &&
                   UserId.Equals(t.UserId) &&
                   ClassId.Equals(t.ClassId) && SchoolId.Equals(t.SchoolId) && DistrictId.Equals(t.DistrictId);
        }

        public override int GetHashCode()
        {
            return DistrictTermId*12 + VirtualTestId*23 + ClassId*34 + SchoolId.GetValueOrDefault(0)*45 + DistrictId*56 +
                   UserId*67;
        }
    }
}