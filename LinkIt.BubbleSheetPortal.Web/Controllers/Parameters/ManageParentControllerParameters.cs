using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.ManageParent;
using LinkIt.BubbleSheetPortal.Web.ViewModels.Validators;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class ManageParentControllerParameters
    {
        public IManageParentService ManageParentService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public UserService UserService { get; set; }
        public ClassListService ClassListService { get; set; }
        public CreateParentModelValidator CreateParentModelValidator { get; set; }
        public StudentService StudentService { get; set; }
        public UpdateParentModelValidator UpdateParentModelValidator { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        
        public EmailService EmailService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public ParentService ParentService { get; set; }
        
    }
}
