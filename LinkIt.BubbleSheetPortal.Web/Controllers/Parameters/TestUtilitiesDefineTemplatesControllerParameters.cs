using LinkIt.BubbleSheetPortal.Services;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class TestUtilitiesDefineTemplatesControllerParameters
    {
        public DistrictService DistrictService { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public UserService UserService { get; set; }
        public DataLockerTemplateService DataLockerTemplateService { get; set; }
        public VirtualTestCustomScoreService VirtualTestCustomScoreService { get; set; }
        public VirtualTestCustomSubScoreService VirtualTestCustomSubScoreService { get; set; }
        public DataLockerService DataLockerService { get; set; }
        public DownloadPdfService DownloadPdfService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public VirtualTestCustomMetaDataService VirtualTestCustomMetaDataService { get; set; }
        public ItemTagService ItemTagService { get; set; }
        public ConversionSetService ConversionSetService { get; set; }
    }
}
