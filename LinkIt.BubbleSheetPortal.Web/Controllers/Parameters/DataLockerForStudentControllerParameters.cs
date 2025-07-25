using LinkIt.BubbleSheetPortal.Models;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Services.CodeGen;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using S3Library;
using System.Configuration;

namespace LinkIt.BubbleSheetPortal.Web.Controllers.Parameters
{
    public class DataLockerForStudentControllerParameters
    {
        public DataLockerForStudentService DataLockerForStudentService { get; set; }
        public QTITestClassAssignmentService QTITestClassAssignmentServices { get; set; }
        public PreferencesService PreferencesService { get; set; }
        public ConfigurationService ConfigurationService { get; set; }
        public TestCodeGenerator TestCodeGenerator { get; set; }
        public DistrictDecodeService DistrictDecodeService { get; set; }
        public StateService StateService { get; set; }

        public S3Service S3Service;
        public string DTLBucket
        {
            get
            {
                return LinkitConfigurationManager.GetS3Settings().DTLBucket;//get from Vault
            }
        }
        public string DTLFolder
        {
            get
            {
                return ConfigurationManager.AppSettings[ContaintUtil.AppSettingDTLFolderName];
            }
        }
    }
}
