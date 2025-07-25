using FluentValidation.Results;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace LinkIt.BubbleSheetPortal.Web.ViewModels.AssignSurvey
{
    [Serializable]
    public class SurveyAssignResultViewModel
    {
        public int Id { get; set; }
        public int Status { get; set; }
        public DateTime? AssignmentDate { get; set; }
        public string Respondent { get; set; }
        public string Email { get; set; }
        public DateTime? ResponseDate { get; set; }
        public string ShortLink { get; set; }
        public string Code { get; set; }
        public int? StudentId { get; set; }
    }
}
