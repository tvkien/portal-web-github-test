using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class LearningLibraryControllerParameters
    {
        public DistrictService DistrictService { get; set; }
        public LessonService LessonService { get; set; }
        public LessonContentTypeService LessonContentTypeService { get; set; }
        public LessonSubjectService LessonSubjectService { get; set; }
        public GradeService GradeService { get; set; }
        public UserSchoolService UserSchoolService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
    }
}