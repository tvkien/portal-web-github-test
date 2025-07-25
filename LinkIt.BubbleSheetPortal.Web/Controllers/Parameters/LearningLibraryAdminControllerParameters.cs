using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;
using FluentValidation;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class LearningLibraryAdminControllerParameters
    {
        public DistrictService DistrictService { get; set; }
        public LessonService LessonService { get; set; }
        public LessonContentTypeService LessonContentTypeService { get; set; }
        public LessonSubjectService LessonSubjectService { get; set; }
        public GradeService GradeService { get; set; }
        public MasterStandardResourceService MasterStandardService { get; set; }
        public IValidator<ResourceViewModel> ResourceViewModelValidator { get; set; }
        public LessonFileTypeService LessonFileTypeService { get; set; }
        public StateService StateService { get; set; }
        public LessonProviderService LessonProviderService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }

    }
}