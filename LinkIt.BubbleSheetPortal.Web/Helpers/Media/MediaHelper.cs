using System;
using System.IO;
using System.Web;
using System.Collections.Generic;
using dotless.Core.Utils;
using LinkIt.BubbleSheetPortal.Common;
using LinkIt.BubbleSheetPortal.Services;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using S3Library;
using LinkIt.BubbleSheetPortal.Web.Security;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.Media
{
    public class MediaHelper
    {
        public static AccessMediaResult UploadPassageMedia(MediaModel model, IS3Service s3Service)
        {
            try
            {
                if (model.PostedFile == null || model.PostedFile.ContentLength == 0)
                {
                    return new AccessMediaResult
                    {
                        Success = "false",
                        ErrorMessage = "Input file is empty."
                    };
                }

                model.FileName = LinkitPath.GetFileName(model.PostedFile.FileName);
                model.FileName = model.FileName.AddTimestampToFileName();

                //if (model.MediaType == MediaType.Audio || model.MediaType == MediaType.Image)
                //    StoreOnFileSystem(model.FileSystemPassageMedia, model.FileName, model.PostedFile);

                var result = new AccessMediaResult
                {
                    Success = "true",
                    MediaPath = model.FileSystemPassageMediaRelativePath + "/" + model.FileName
                };

                // Upload file to S3 
                //if (Util.UploadTestItemMediaToS3 || model.MediaType == MediaType.Video)//alway upload to S3 now
                {
                    var s3Result = UploadPassageMediaToS3(model, s3Service);
                    if (s3Result == null || !s3Result.IsSuccess)
                    {
                        result.Success = "false";
                        result.ErrorMessage = "Upload file to S3 fail.";
                        return result;
                    }

                    if (model.MediaType == MediaType.Video)
                    {
                        result.MediaPath = s3Result.ReturnValue;
                    }

                }

                return result;
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                var errorMessage = "An error occurs.";
                if (ex.Message == "Illegal characters in path.")
                    errorMessage = "Invalid file name.";
                return new AccessMediaResult
                {
                    Success = "false",
                    ErrorMessage = errorMessage
                };
            }
        }

        public static AccessMediaResult UploadTestMedia(MediaModel model, IS3Service s3Service)
        {
            try
            {
                if (model.PostedFile == null || model.PostedFile.ContentLength == 0)
                {
                    return new AccessMediaResult
                    {
                        Success = "false",
                        ErrorMessage = "Input file is empty."
                    };
                }

                model.FileName = LinkitPath.GetFileName(model.PostedFile.FileName);
                model.FileName = StringUtils.RemoveSpecialCharacters(model.FileName);
                model.FileName = model.FileName.AddTimestampToFileName();
                
                //No store media files on web server any more
                //No store media files on web server any more
                //if (model.MediaType == MediaType.Audio || model.MediaType == MediaType.Image)
                //    StoreOnFileSystem(model.FileSystemTestMedia, model.FileName, model.PostedFile);

                var result = new AccessMediaResult
                {
                    Success = "true",
                    MediaPath = model.FileSystemTestMediaRelativePath + "/" + model.FileName
                };

                // Upload file to S3 
                //if (Util.UploadTestItemMediaToS3 || model.MediaType == MediaType.Video)//alsways upload to S3 now
                {
                    var s3Result = UploadTestMediaToS3(model, s3Service);
                    if (s3Result == null || !s3Result.IsSuccess)
                    {
                        result.Success = "false";
                        result.ErrorMessage = "Upload file to S3 fail.";
                        return result;
                    }

                    if (model.MediaType == MediaType.Video) 
                    result.MediaPath = s3Result.ReturnValue;
                }

                return result;
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                var errorMessage = "An error occurs.";
                if (ex.Message == "Illegal characters in path.")
                    errorMessage = "Invalid file name.";
                return new AccessMediaResult
                {
                    Success = "false",
                    ErrorMessage = errorMessage
                };
            }
        }

        public static AccessMediaResult UploadSectionMedia(MediaModel model, IS3Service s3Service)
        {
            try
            {
                if (model.PostedFile == null || model.PostedFile.ContentLength == 0)
                {
                    return new AccessMediaResult
                    {
                        Success = "false",
                        ErrorMessage = "Input file is empty."
                    };
                }

                model.FileName = LinkitPath.GetFileName(model.PostedFile.FileName);
                //Add timestamp to file name to avoid duplicating
                model.FileName = model.FileName.AddTimestampToFileName();
                //if (model.MediaType == MediaType.Audio)
                //    StoreOnFileSystem(model.FileSystemSectionMedia, model.FileName, model.PostedFile);

                var result = new AccessMediaResult
                {
                    Success = "true",
                    MediaPath = model.FileSystemSectionMediaRelativePath + "/" + model.FileName
                };

                // Upload file to S3 
                //if (Util.UploadTestItemMediaToS3 || model.MediaType == MediaType.Video)//now section audio file is always uploaded to S3
                {
                    var s3Result = UploadSectionMediaToS3(model, s3Service);
                    if (s3Result == null || !s3Result.IsSuccess)
                    {
                        result.Success = "false";
                        result.ErrorMessage = "Upload file to S3 fail.";
                        return result;
                    }

                    //if (model.MediaType == MediaType.Audio) result.MediaPath = s3Result.ReturnValue;
                }

                return result;
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                var errorMessage = "An error occurs.";
                if (ex.Message == "Illegal characters in path.")
                    errorMessage = "Invalid file name.";
                return new AccessMediaResult
                {
                    Success = "false",
                    ErrorMessage = errorMessage
                };
            }
        }

        public static AccessMediaResult UploadQuestionGroupFile(MediaModel model, IS3Service s3Service)
        {
            try
            {
                if (model.PostedFile == null || model.PostedFile.ContentLength == 0)
                {
                    return new AccessMediaResult
                    {
                        Success = "false",
                        ErrorMessage = "Input file is empty."
                    };
                }

                model.FileName = LinkitPath.GetFileName(model.PostedFile.FileName);
                //Add timestamp to file name to avoid duplicating
                model.FileName = model.FileName.AddTimestampToFileName();

                var result = new AccessMediaResult
                {
                    Success = "true",
                    MediaPath = model.FileSystemQuestionGroupMediaRelativePath + "/" + model.FileName
                };

                var s3Result = UploadQuestionGroupMediaToS3(model, s3Service);
                if (s3Result == null || !s3Result.IsSuccess)
                {
                    result.Success = "false";
                    result.ErrorMessage = "Upload file to S3 fail.";
                    return result;
                }

                return result;
            }
            catch (Exception ex)
            {
                PortalAuditManager.LogException(ex);
                var errorMessage = "An error occurs.";
                if (ex.Message == "Illegal characters in path.")
                    errorMessage = "Invalid file name.";
                return new AccessMediaResult
                {
                    Success = "false",
                    ErrorMessage = errorMessage
                };
            }
        }

        private static void StoreOnFileSystem(string folder, string fileName, HttpPostedFileBase file)
        {
            folder = Path.GetFullPath(folder);
            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }

            var filePath = folder + "/" + fileName;
            filePath = Path.GetFullPath(filePath);

            file.SaveAs(filePath);
        }

        private static S3Result UploadPassageMediaToS3(MediaModel model, IS3Service s3Service)
        {
            var filePath = model.S3PassageMedia + "/" + model.FileName;
            filePath = filePath.Replace("//", "/");
            string contentType = string.Empty;
            if (model.MediaType == MediaType.Image)
            {
                if (Path.GetExtension(model.FileName).ToLower() == ".svg")
                {
                    contentType = "image/svg+xml";
                }
            }
            var result = s3Service.UploadRubricFile(model.UpLoadBucketName, filePath, model.PostedFile.InputStream,true, contentType);
            if (result.IsSuccess) result.ReturnValue = GetS3DownloadPath(model, result.ReturnValue);

            return result;
        }

        private static S3Result UploadTestMediaToS3(MediaModel model, IS3Service s3Service)
        {
            var filePath = model.S3TestMedia + "/" + model.FileName;
            filePath = filePath.Replace("//", "/");

            string contentType = string.Empty;
            if (model.MediaType == MediaType.Image)
            {
                if (Path.GetExtension(model.FileName).ToLower() == ".svg")
                {
                    contentType = "image/svg+xml";
                }
            }

            var result = s3Service.UploadRubricFile(model.UpLoadBucketName, filePath, model.PostedFile.InputStream,true, contentType);
            if (result.IsSuccess) result.ReturnValue = GetS3DownloadPath(model, result.ReturnValue);

            return result;
        }
        public static S3Result UploadTestMediaToS3(MediaModel model,Stream inputStream, IS3Service s3Service)
        {
            var filePath = model.S3TestMedia + "/" + model.FileName;
            filePath = CorrectSlashInS3Url(filePath);
            var result = s3Service.UploadRubricFile(model.UpLoadBucketName, filePath, inputStream);
            if (result.IsSuccess) result.ReturnValue = GetS3DownloadPath(model, result.ReturnValue);

            return result;
        }

        private static string GetS3DownloadPath(MediaModel model, string s3ReturnValue)
        {
            var result = string.Format("{0}/{1}/{2}", model.S3Domain, model.DownLoadBucketName, s3ReturnValue);
            result = CorrectSlashInS3Url(result);
            return result;
        }

        public static string CorrectSlashInS3Url(string url)
        {
            if (string.IsNullOrWhiteSpace(url)) return string.Empty;
            var arr = url.Split(new[] {"/"}, StringSplitOptions.RemoveEmptyEntries);
            var result = string.Empty;
            var index = 0;
            var format = "{0}/{1}";
            foreach (var s in arr)
            {
                index++;
                if (index == 1)
                {
                    result = s.Contains("http") ? string.Format(format, s, result) : s;
                    continue;
                }

                result = string.Format(format, result, s);
            }

            return result;
        }

        //For image and audio (access directly from S3)
        public static string GetS3LinkForItemMedia(MediaModel model, string mediaItem)
        {
            //mediaItem looks like : ItemSet_8752/eagle.png
            var result = string.Empty;
            if (string.IsNullOrEmpty(model.AUVirtualTestFolder))
            {
                result = string.Format("{0}/{1}", UrlUtil.GenerateS3Subdomain(model.S3Domain,model.UpLoadBucketName).RemoveEndSlash(), mediaItem.RemoveStartSlash());
            }
            else
            {
                result = string.Format("{0}/{1}/{2}", UrlUtil.GenerateS3Subdomain(model.S3Domain, model.UpLoadBucketName).RemoveEndSlash(),model.AUVirtualTestFolder.RemoveEndSlash().RemoveStartSlash(), mediaItem.RemoveStartSlash());
            }
             
            return result;
            //result looks like https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_8752/eagle.png
        }
        //For image and audio (access directly from S3)
        public static string GetS3LinkForPassageMedia(MediaModel model, string s3ReturnValue)
        {
            var result = string.Format("{0}{1}{2}{3}", model.S3Domain, model.UpLoadBucketName, model.AUVirtualTestROFolder, s3ReturnValue);
            return result;
            //result look likes https://s3.amazonaws.com/testitemmedia/Vina/ItemSet_8752/eagle.png
        }

        private static S3Result UploadSectionMediaToS3(MediaModel model, IS3Service s3Service)
        {
            var filePath = model.S3SectionMedia + "/" + model.FileName;
            filePath = filePath.Replace("//", "/");
            var result = s3Service.UploadRubricFile(model.UpLoadBucketName, filePath, model.PostedFile.InputStream);
            if (result.IsSuccess) result.ReturnValue = GetS3DownloadPath(model, result.ReturnValue);

            return result;
        }

        private static S3Result UploadQuestionGroupMediaToS3(MediaModel model, IS3Service s3Service)
        {
            var filePath = model.S3QuestionGroupMedia + "/" + model.FileName;
            filePath = filePath.Replace("//", "/");
            var result = s3Service.UploadRubricFile(model.UpLoadBucketName, filePath, model.PostedFile.InputStream);
            if (result.IsSuccess) result.ReturnValue = GetS3DownloadPath(model, result.ReturnValue);

            return result;
        }

        public static List<string> GetVideoExtensions()
        {
            return new List<string> { ".WAV", ".AVI", ".MP4", ".WMV" };
        }

    }
}
