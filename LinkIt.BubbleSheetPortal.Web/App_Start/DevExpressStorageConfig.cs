using Amazon.S3;
using S3Library;

namespace LinkIt.BubbleSheetPortal.Web.App_Start
{
    // https://documentation.devexpress.com/#AspNet/DevExpressWebASPxBinaryImage_BinaryStorageModetopic
    public class DevExpressStorageConfig
    {
        public static void Initialize()
        {
            DevExpress.Web.ASPxClasses.BinaryStorageConfigurator.Mode =
                DevExpress.Web.ASPxClasses.BinaryStorageMode.Custom;
            DevExpress.Web.ASPxClasses.BinaryStorageConfigurator.RegisterCustomStorageStrategy(
                new DevExpressStaticImageStrategy(new S3Service(new AmazonS3Client())));            
        }        
    }
}
