using System;
using Amazon.S3;
using Amazon.S3.Model;
using System.IO;
using S3Library.Services;
using S3Library.Utilities;
using Amazon.S3.IO;

namespace S3Library
{
    public class S3Service : IS3Service
    {
        private const string portalNotificationKey = "Portal-Notification-Url";
        private int _timeout = 5; //5 minutes

        private readonly IAmazonS3 _s3Client;

        public S3Service(IAmazonS3 s3Client)
        {
            _s3Client = s3Client;
        }

        private void PutToS3(string keyName, string bucketName, System.IO.Stream Stream, TimeSpan timeOut)
        {
            keyName = Utility.RemoveStartSlash(keyName);
            //upload file
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = Stream
            };

            request.CannedACL = S3CannedACL.PublicRead;
            request.StorageClass = S3StorageClass.Standard;
            request.Timeout = timeOut;
            _s3Client.PutObject(request);
        }

        public S3Result UploadRubricFile(string bucketName, string keyName, System.IO.Stream stream,
            bool isPublic = true, string contentType = null)
        {
            keyName = Utility.RemoveStartSlash(keyName);
            var s3CannedACL = isPublic ? S3CannedACL.PublicRead : S3CannedACL.Private;
            return UploadRubricFile(bucketName, keyName, stream, s3CannedACL, contentType);
        }

        private S3Result UploadRubricFile(string bucketName, string keyName, System.IO.Stream stream,
            S3CannedACL s3CannedACL, string contentType = null)
        {
            try
            {
                //upload file
                var request = new PutObjectRequest();
                request.Key = keyName;
                request.InputStream = stream;
                request.BucketName = bucketName;
                request.CannedACL = s3CannedACL;
                request.StorageClass = S3StorageClass.Standard;
                request.Timeout = TimeSpan.FromMinutes(_timeout);
                if (!string.IsNullOrEmpty(contentType))
                {
                    request.ContentType = contentType;
                }
                _s3Client.PutObject(request);
                return new S3Result()
                {
                    ErrorCode = string.Empty,
                    ErrorMessage = string.Empty,
                    IsSuccess = true,
                    ReturnValue = keyName
                };
            }
            catch (AmazonS3Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = ex.ErrorCode,
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
            catch (Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = "",
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
        }

        public S3Result UploadRubricFile(string bucketName, string keyName, System.IO.Stream Stream)
        {
            keyName = Utility.RemoveStartSlash(keyName);
            try
            {
                PutToS3(keyName, bucketName, Stream, TimeSpan.FromMinutes(_timeout));
                return new S3Result()
                {
                    ErrorCode = string.Empty,
                    ErrorMessage = string.Empty,
                    IsSuccess = true,
                    ReturnValue = keyName
                };
            }
            catch (AmazonS3Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = ex.ErrorCode,
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
            catch (Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = "",
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
        }

        public void UploadFile(string bucketName, string keyName, Stream content)
        {
            var request = new PutObjectRequest
            {
                BucketName = bucketName,
                Key = keyName,
                InputStream = content
            };
            _s3Client.PutObject(request);
        }

        public S3DownloadResult DownloadFile(string bucketName, string keyName)
        {
            keyName = Utility.CleanUrl(keyName);
            try
            {
                var request = new GetObjectRequest
                {
                    BucketName = bucketName,
                    Key = keyName
                };

                using (var response = _s3Client.GetObject(request))
                {
                    var binaryData = ReadFully(response.ResponseStream);
                    return new S3DownloadResult
                    {
                        ErrorCode = string.Empty,
                        ErrorMessage = string.Empty,
                        IsSuccess = true,
                        ReturnStream = binaryData
                    };
                }
            }
            catch (AmazonS3Exception ex)
            {
                return new S3DownloadResult
                {
                    ErrorCode = ex.ErrorCode,
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnStream = null
                };
            }
            catch (Exception ex)
            {
                return new S3DownloadResult
                {
                    ErrorCode = string.Empty,
                    ErrorMessage = ex.ToString(),
                    IsSuccess = false,
                };
            }
        }

        public string GetPublicUrl(string bucketName, string keyName)
        {
            keyName = Utility.RemoveStartSlash(keyName);
            var request = new GetObjectRequest();
            request.BucketName = bucketName;
            request.Key = keyName;
            bool isExisted = false;

            try
            {
                var response = _s3Client.GetObject(request);
                isExisted = (response.ContentLength > 0);
            }
            catch (Exception e)
            {
                // Do nothing
            }
            if (isExisted)
            {
                return _s3Client.GetPreSignedURL(new GetPreSignedUrlRequest()
                {
                    BucketName = bucketName,
                    Key = keyName,
                    Expires = DateTime.Now.AddMinutes(_timeout)
                });
            }
            else
            {
                return string.Empty;
            }
        }

        public S3PortalLinkDTO GetS3PortalLinkByKey(string portalKey, string s3ConnectionString)
        {
            var repo = new S3PortalLinkService(s3ConnectionString);
            var spl = repo.GetS3PortalLinkByKey(portalKey);
            if (spl != null)
            {
                return new S3PortalLinkDTO
                {
                    S3PortalLinkID = spl.S3PortalLinkID,
                    ServiceType = (int)spl.ServiceType,
                    DistrictID = spl.DistrictID,
                    BucketName = spl.BucketName,
                    FilePath = spl.FilePath,
                    DateCreated = (DateTime)spl.DateCreated,
                    PortalKey = spl.PortalKey
                };
            }
            return null;
        }

        public ConfigurationDTO GetConfiguration(string name, int type, string s3ConnectionString)
        {
            S3Library.Entities.Configuration config =
                new ConfigurationService(s3ConnectionString).GetByNameAndType(name, type);
            if (config != null)
            {
                return new ConfigurationDTO
                {
                    Name = config.Name,
                    Type = config.Type,
                    Value = config.Value
                };
            }
            return null;
        }

        private byte[] ReadFully(Stream stream, int initialLength = -1)
        {
            if (initialLength < 1)
            {
                initialLength = 32768;
            }

            var buffer = new byte[initialLength];
            var read = 0;
            int chunk;
            while ((chunk = stream.Read(buffer, read, buffer.Length - read)) > 0)
            {
                read += chunk;
                if (read == buffer.Length)
                {
                    int nextByte = stream.ReadByte();
                    if (nextByte == -1)
                    {
                        return buffer;
                    }

                    var newBuffer = new byte[buffer.Length * 2];
                    Array.Copy(buffer, newBuffer, buffer.Length);
                    newBuffer[read] = (byte)nextByte;
                    buffer = newBuffer;
                    read++;
                }
            }

            var ret = new byte[read];
            Array.Copy(buffer, ret, read);
            return ret;
        }

        public S3Result DeleteFile(string bucketName, string folder, string fileName)
        {
            try
            {
                //delete file
                string filePath = string.Format("{0}/{1}", folder, fileName);
                DeleteObjectRequest request = new DeleteObjectRequest();
                request.BucketName = bucketName;
                request.Key = filePath;
                _s3Client.DeleteObject(request);


                return new S3Result()
                {
                    ErrorCode = string.Empty,
                    ErrorMessage = string.Empty,
                    IsSuccess = true,
                    ReturnValue = filePath
                };
            }
            catch (AmazonS3Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = ex.ErrorCode,
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
            catch (Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = "",
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
        }

        public S3Result UploadResourceFile(string bucketName, string keyName, System.IO.Stream Stream,
            int timeoutMinutes)
        {
            var tagsSplit = keyName.Split(new[] { '/' }, StringSplitOptions.RemoveEmptyEntries);
            keyName = string.Join("/", tagsSplit);
            try
            {
                PutToS3(keyName, bucketName, Stream, TimeSpan.FromMinutes(timeoutMinutes));
                return new S3Result()
                {
                    ErrorCode = string.Empty,
                    ErrorMessage = string.Empty,
                    IsSuccess = true,
                    ReturnValue = keyName
                };
            }
            catch (AmazonS3Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = ex.ErrorCode,
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
            catch (Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = "",
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
        }

        public string GetReferenceContent(string bucketName, string keyName)
        {
            string contents = null;
            keyName = Utility.RemoveStartSlash(keyName);
            var request = new GetObjectRequest();
            request.BucketName = bucketName;
            request.Key = keyName;

            try
            {
                var response = _s3Client.GetObject(request);
                if (response.ResponseStream != null)
                {
                    using (StreamReader reader = new StreamReader(response.ResponseStream))
                    {
                        contents = reader.ReadToEnd();

                    }
                }
            }
            catch (Exception ex)
            {
                return null;
            }
            return contents;
        }

        public S3Result DeleteFile(string bucketName, string filePath)
        {
            try
            {
                //delete file
                DeleteObjectRequest request = new DeleteObjectRequest();
                request.BucketName = bucketName;
                request.Key = filePath;
                _s3Client.DeleteObject(request);


                return new S3Result()
                {
                    ErrorCode = string.Empty,
                    ErrorMessage = string.Empty,
                    IsSuccess = true,
                    ReturnValue = filePath
                };
            }
            catch (AmazonS3Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = ex.ErrorCode,
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
            catch (Exception ex)
            {
                return new S3Result()
                {
                    ErrorCode = "",
                    ErrorMessage = ex.Message,
                    IsSuccess = false,
                    ReturnValue = ""
                };
            }
        }

        public void CopyFile(string bucketName, string sourceKey, string targetKey)
        {
            var fileInfo = new S3FileInfo(_s3Client, bucketName, sourceKey);
            fileInfo.CopyTo(bucketName, targetKey);
        }
    }
}
