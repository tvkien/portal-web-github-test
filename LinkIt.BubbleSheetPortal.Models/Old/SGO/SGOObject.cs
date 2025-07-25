using System;

namespace LinkIt.BubbleSheetPortal.Models.SGO
{
    public class SGOObject
    {
        public int SGOID { get; set; }
        public string Name { get; set; }
        public int TargetScoreType { get; set; }
        public int OwnerUserID { get; set; }
        public DateTime CreatedDate { get; set; }
        public DateTime UpdatedDate { get; set; }
        public int? ApproverUserID { get; set; }
        public int SGOStatusID { get; set; }

        public DateTime? StartDate { get; set; }

        public DateTime? EndDate { get; set; }

        public string SchoolIDs { get; set; }
        public string ClassIDs { get; set; }
        public string GradeIDs { get; set; }

        public int Version { get; set; }

        public bool IsArchive { get; set; }

        public int DistrictID { get; set; }

        public string Feedback { get; set; }

        public string AdminComment { get; set; }

        public string TeacherComment { get; set; }

        public string EducatorComment { get; set; }

        public DateTime? GenerateResultDate { get; set; }

        public int Type { get; set; }

        public string RationaleUnstructuredScoring { get; set; }
        public string AttachUnstructuredScoringUrl { get; set; }
        public string AttachUnstructuredProgressUrl { get; set; }
        public string TotalTeacherSGOScoreCustom { get; set; }

        public DateTime? PreparationApprovedDate { get; set; }
        public string SGONote { get; set; }
    }
}