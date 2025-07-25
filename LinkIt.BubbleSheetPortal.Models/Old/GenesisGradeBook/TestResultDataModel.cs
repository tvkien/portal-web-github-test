using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace LinkIt.BubbleSheetPortal.Models.GenesisGradeBook
{
    public class TestResultDataModel
    {
        public int DistrictId { get; set; }
        public int UserId { get; set; }
        public int RoleId { get; set; }
        public int? Schoolid { get; set; }
        public string TeacherName { get; set; }
        public int? ClassId { get; set; }
        public string StudentName { get; set; }
        public int? VirtualTestId { get; set; }
        public int TermId { get; set; }
        public bool IsShowExported { get; set; }
        public int PageIndex { get; set; }
        public int PageSize { get; set; }
        public string SortColumns { get; set; }
        public bool IsStudentInformationSystem { get; set; }
        public string DateCompare { get; set; }
        public string GeneralSearch { get; set; }
    }
}
