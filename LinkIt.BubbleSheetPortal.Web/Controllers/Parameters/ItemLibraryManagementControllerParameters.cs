using FluentValidation;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class ItemLibraryManagementControllerParameters
    {
        public QtiBankService QTIBankServices { get; set; }
        public QtiBankDistrictService QtiBankDistrictServices { get; set; }
        public QtiBankSchoolService QtiBankSchoolServices { get; set; }
        public DistrictService DistrictServices { get; set; }
        public SchoolService SchoolServices { get; set; }

        public IValidator<QtiBankPublishToDistrictViewModel> QtiBankPublishToDistrictViewModelValidator { get; set; }
        public IValidator<QtiBankPublishToSchoolViewModel> QtiBankPublishToSchoolViewModelValidator { get; set; }
    }
}