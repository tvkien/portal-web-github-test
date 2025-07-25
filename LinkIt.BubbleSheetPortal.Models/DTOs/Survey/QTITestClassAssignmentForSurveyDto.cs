using LinkIt.BubbleSheetPortal.Common.Enum;
using System;

namespace LinkIt.BubbleSheetPortal.Models.DTOs.Survey
{
    public class QTITestClassAssignmentForSurveyDto
    {
        public DateTime? AssignmentDate { get; set; }
        public string TestName { get; set; }
        public string Identity
        {
            get
            {
                switch (SurveyAssignmentType)
                {
                    case (int)SurveyAssignmentTypeEnum.PrivateAnonymous:
                    case (int)SurveyAssignmentTypeEnum.PublicAnonymous:
                        return "Hidden";
                    case (int)SurveyAssignmentTypeEnum.PrivateIndividualized:
                    case (int)SurveyAssignmentTypeEnum.PublicIndividualized:
                        return "Shown";
                    default:
                        return string.Empty;
                        break;
                }
            }
        }
        public string Code { get; set; }
        public string Status { get; set; }
        public string RedirectUrl { get; set; }
        public bool IsValid { get; set; } = true;
        public string AssignmentGUID { get; set; }
        public int QTITestClassAssignmentId { get; set; }
        public int? QTIOnlineTestSessionId { get; set; }
        public int ClassId { get; set; }
        public int SurveyAssignmentType { get; set; }
        public int StudentId { get; set; }
        public string ErrorMsg { get; set; }
    }
}
