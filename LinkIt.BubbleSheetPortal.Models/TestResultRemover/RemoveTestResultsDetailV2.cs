using System;

namespace LinkIt.BubbleSheetPortal.Models
{
    public class RemoveTestResultsDetailV2
    {
        public int DistrictId { get; set; }
        public string SchoolIds { get; set; }
        public string CategoryIds { get; set; }
        public string GradeIds { get; set; }
        public string SubjectNames { get; set; }
        public int TermId { get; set; }
        public int ClassId { get; set; }
        public string TeacherName { get; set; }
        public string StudentName { get; set; }
        public DateTime? FromResultDate { get; set; }
        public DateTime? ToResultDate { get; set; }
        public DateTime? FromCreatedDate { get; set; }
        public DateTime? ToCreatedDate { get; set; }
        public DateTime? FromUpdatedDate { get; set; }
        public DateTime? ToUpdatedDate { get; set; }
        public string VirtualTestName { get; set; }

        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int PageSize { get; set; }
        public int StartIndex { get; set; }
        public string SortColumns { get; set; }
        public string GeneralSearch { get; set; }
        public bool HasStudentGeneralSearch { get; set; } = true;
    }
}
