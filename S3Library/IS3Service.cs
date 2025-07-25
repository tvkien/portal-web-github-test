using System.IO;

namespace S3Library
{
    public interface IS3Service
    {
        void UploadFile(string bucketName, string keyName, Stream content);
        S3DownloadResult DownloadFile(string bucketName, string keyName);
        string GetPublicUrl(string bucketName, string keyName);
        S3Result UploadRubricFile(string bucketName, string keyName, System.IO.Stream Stream);
        S3Result UploadRubricFile(string bucketName, string keyName, System.IO.Stream stream,
            bool isPublic = true, string contentType = null);
        S3Result UploadResourceFile(string bucketName, string keyName, System.IO.Stream Stream,
            int timeoutMinutes);
        S3Result DeleteFile(string bucketName, string folder, string fileName);
        S3Result DeleteFile(string bucketName, string filePath);
        S3PortalLinkDTO GetS3PortalLinkByKey(string portalKey, string s3ConnectionString);
        ConfigurationDTO GetConfiguration(string name, int type, string s3ConnectionString);
        string GetReferenceContent(string bucketName, string keyName);
        void CopyFile(string bucketName, string sourceKey, string targetKey);
    }
}
