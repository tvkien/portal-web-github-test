using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class SGOManageControllerParameters
    {
        public RaceService RaceService { get; set; }

        public ProgramService ProgramService { get; set; }

        public DistrictTermService DistrictTermService { get; set; }

        public ClassService ClassService { get; set; }

        public ClassStudentCustomService ClassStudentCustomService { get; set; }
        
        public StudentService StudentService { get; set; }

        public SGOObjectService SGOObjectService { get; set; }

        public SGOStudentFilterService SGOStudentFilterService { get; set; }

        public DistrictDecodeService DistrictDecodeService { get; set; }

        public SGOStudentService SgoStudentService { get; set; }

        public SGOGroupService SgoGroupService { get; set; }

        public SGODataPointService SgoDataPointService { get; set; }        

        public GradeService GradeService { get; set; }

        public SGOMilestoneService SGOMilestoneService { get; set; }

        public ConfigurationService ConfigurationService { get; set; }

        public UserService UserService { get; set; }

        public SGOSelectDataPointService SgoSelectDataPointService { get; set; }
        public UserSchoolService UserSchoolServices { get; set; }
        public StudentService StudentServices { get; set; }

        public VulnerabilityService VulnerabilityService { get; set; }
        public VirtualTestCustomScoreService VirtualTestCustomScoreService { get; set; }
        public VirtualTestCustomSubScoreService VirtualTestCustomSubScoreService { get; set; }
        public SGODataPointClusterScoreService SgoDataPointClusterScoreService { get; set; }
        public DataLockerService DataLockerService { get; set; }
        public PreferencesService PreferencesService { get; set; }
    }
}
