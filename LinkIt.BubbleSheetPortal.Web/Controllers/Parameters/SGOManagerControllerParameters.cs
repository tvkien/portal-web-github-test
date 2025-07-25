using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class SGOManagerControllerParameters
    {
        public RaceService RaceService { get; set; }

        public ProgramService ProgramService { get; set; }

        public DistrictTermService DistrictTermService { get; set; }

        public ClassService ClassService { get; set; }

        public ClassStudentCustomService ClassStudentCustomService { get; set; }

        public StudentProgramService StudentProgramService { get; set; }

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
    }
}