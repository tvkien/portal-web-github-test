using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class QTIItemControllerParameters
    {
        public QTIITemService QTIITemServices { get; set; }
        public VirtualTestService VirtualTestServices { get; set; }
        public VirtualQuestionService VirtualQuestionServices { get; set; }
        public BankService BankServices { get; set; }
        public QtiGroupService QtiGroupService { get; set; }
        public QTIRefObjectService PassageService { get; set; }
        public QTIRefObjectHistoryService PassageHistoryService { get; set; }
        public MasterStandardService MasterStandardService { get; set; }
        public VirtualSectionService VirtualSectionService { get; set; }
        public VirtualSectionQuestionService VirtualSectionQuestionService { get; set; }
        public Qti3pPassageService Qti3pPassageService { get; set; }
        public QtiBankService QtiBankService { get; set; }
        public DistrictService DistrictService { get; set; }
        public StateService StateService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }

        public QTIITemService QtiItemService { get; set; }
        public QTI3pItemToPassageService QTI3pItemToPassageService { get; set; }
        public DataFileUploadLogService DataFileUploadLogService { get; set; }
        public VulnerabilityService VulnerabilityService { get; set; }
        public QTI3Service QTI3Service { get; set; }
        public DownloadPdfService DownloadPdfService { get; set; }
    }
}
