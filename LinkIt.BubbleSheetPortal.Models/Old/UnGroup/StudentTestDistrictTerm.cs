using System;
using Envoc.Core.Shared.Extensions;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class StudentTestDistrictTerm
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

        public int StudentId { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }

        public int TakenTestClassId { get; set; }
        public int TakenTestDistrictTermId { get; set; }
        public int TakenTestSchoolId { get; set; }
        public int TakenTestTeacherId { get; set; }
    }
}