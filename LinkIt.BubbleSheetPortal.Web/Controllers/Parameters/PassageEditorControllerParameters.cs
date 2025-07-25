using FluentValidation;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.ViewModels;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class PassageEditorControllerParameters
    {
        public QTIRefObjectService QtiRefObjectService { get; set; }
        public QTIRefObjectHistoryService QtiRefObjectHistoryService { get; set; }

        public IValidator<EditPassageViewModel> EditPassageViewModelValidator { get; set; }
        public GradeService GradeService { get; set; }
        public StateService StateService { get; set; }
        public QTI3pTextTypeService QTI3pTextTypeService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public QTI3Service QiQti3Service { get; set; }

        public QtiItemRefObjectService QtiItemRefObjectService { get; set; }

        public VirtualTestService VirtualTestService { get; set; }

        public UserService UserService { get; set; }
    }
}
