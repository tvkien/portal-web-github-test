using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Data.Repositories;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.BusinessObjects;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class SGOSelectDataPointControllerParameters
    {
        public SGOSelectDataPointService SgoSelectDataPointService { get; set; }
        public StateService StateService { get; set; }
        public MasterStandardService MasterStandardService { get; set; }

        public VirtualQuestionTopicService VirtualQuestionTopicService { get; set; }
        public VirtualQuestionLessonOneService VirtualQuestionLessonOneService { get; set; }
        public VirtualQuestionLessonTwoService VirtualQuestionLessonTwoService { get; set; }
        public SubjectService SubjectService { get; set; }
        public GradeService GradeService { get; set; }     
   
        public SGODataPointService SgoDataPointService { get; set; }
        public SGODataPointFilterService SgoDataPointFilterService { get; set; }
        public SGODataPointClusterScoreService SgoDataPointClusterScoreService { get; set; }

        public VirtualTestService VirtualTestService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public DistrictService DistrictService { get; set; }

        public SGOStudentDataPointService SGOStudentDataPointService { get; set; }
        public TestResultService TestResultService { get; set; }
        public TestResultScoreService TestResultScoreService { get; set; }

        public SGOStudentService SGOStudentService { get; set; }

        public DistrictDecodeService DistrictDecodeService { get; set; }

        public SGOObjectService SGOObjectService { get; set; }

        public VirtualTestCustomScoreService VirtualTestCustomScoreService { get; set; }
        public VirtualTestCustomSubScoreService VirtualTestCustomSubScoreService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public VirtualTestVirtualTestCustomScoreService VirtualTestVirtualTestCustomScoreService { get; set; }

        public BankService BankService { get; set; }

        public AchievementLevelSettingService AchievementLevelSettingService { get; set; }
        public CategoriesService DataSetCategoriesService { get; set; }

    }
}
