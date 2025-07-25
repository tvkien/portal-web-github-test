using FluentValidation;
using LinkIt.BubbleService.Models.Test;
using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class GroupPrintingControllerParameters
    {
        public PrintingGroupDataService PrintingGroupDataService { get; set; }

        public GroupPrintingService GroupPrintingService { get; set; }

        public ClassPrintingGroupService ClassPrintingGroupService { get; set; }

        public UserService UserService { get; set; }

        public ClassService ClassService { get; set; }

        public DistrictService DistrictService { get; set; }

        public SchoolService SchoolService { get; set; }

        public DistrictTermService DistrictTermService { get; set; }

        public IValidator<RequestSheet> SheetRequestValidator { get; set; }

        public IValidator<BubbleSheetGroupData> BubbleSheetGroupDataValidator { get; set; }
        
        public TestService TestService { get; set; }

        public BankService BankService { get; set; }

        public BubbleSheetPrintingService BubbleSheetPrintingService { get; set; }

        public ClassUserService ClassUserService { get; set; }

        public VirtualTestService VirtualTestService { get; set; }
        public ClassPrintingGroupService ClassPrintingGroupServices { get; set; }

        public QTITestClassAssignmentService QTITestClassAssignmentServices { get; set; }

        public ClassCustomService ClassCustomService { get; set; }

        public TestAssignmentService TestAssignmentService { get; set; }

        public QuestionOptionsService QuestionOptionsService { get; set; }

        public DistrictDecodeService DistrictDecodeServices { get; set; }
    }
}
