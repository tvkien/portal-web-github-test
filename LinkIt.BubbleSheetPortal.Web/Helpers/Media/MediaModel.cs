using System.Configuration;
using System.Web;
using LinkIt.BubbleSheetPortal.Web.DataScopeManager;
using LinkIt.BubbleSheetPortal.Common;

namespace LinkIt.BubbleSheetPortal.Web.Helpers.Media
{
    public class MediaModel
    {
        public MediaModel()
        {
            UseS3Content = true;
        }
        public MediaType MediaType { get; set; }
        public HttpPostedFileBase PostedFile { get; set; }
        public int ID { get; set; }

        public string FileName { get; set; }

        public string FileSystemPassageMediaRelativePath
        {
            get
            {
                const string format = "/RO/RO_{0}_media";
                var result = string.Format(format, ID);

                return result;
            }
        }

        public string S3PassageMediaRelativePath
        {
            get
            {
                const string format = "RO/RO_{0}_media";
                var result = string.Format(format, ID);

                return result;
            }
        }

        public string FileSystemTestMediaRelativePath
        {
            get
            {
                const string format = "/ItemSet_{0}";
                var result = string.Format(format, ID);

                return result;
            }
        }

        public string S3TestMediaRelativePath
        {
            get
            {
                const string format = "ItemSet_{0}";
                var result = string.Format(format, ID);

                return result;
            }
        }
        
        //public string FileSystemTestMedia
        //{
        //    get
        //    {
        //        const string passageMediaPathFormat = "{0}/{1}";
        //        var result = string.Format(passageMediaPathFormat, FileSystemTestItemMediaPath, FileSystemTestMediaRelativePath);

        //        return result;
        //    }
        //}

        //public string FileSystemPassageMedia
        //{
        //    get
        //    {
        //        const string passageMediaPathFormat = "{0}/{1}";
        //        var result = string.Format(passageMediaPathFormat, FileSystemTestItemMediaPath, FileSystemPassageMediaRelativePath);

        //        return result;
        //    }
        //}

        public string S3TestMedia
        {
            get
            {
                var result = string.Empty;
                if (string.IsNullOrEmpty(AUVirtualTestFolder))
                {
                    result = S3TestMediaRelativePath;
                }
                else
                {
                    result = string.Format("{0}/{1}", AUVirtualTestFolder.RemoveEndSlash(),
                        S3TestMediaRelativePath.RemoveStartSlash());
                }

                return result;
            }
        }

        public string S3PassageMedia
        {
            get
            {
                const string passageMediaPathFormat = "{0}{1}";
                var result = string.Format(passageMediaPathFormat, AUVirtualTestROFolder, S3PassageMediaRelativePath);

                return result;
            }
        }

        //public string FileSystemTestItemMediaPath
        //{
        //    get
        //    {
        //        //var result = ConfigurationManager.AppSettings["TestItemMediaPath"];
        //        //return result;
        //        return string.Empty;
        //    }
        //}

        public string AUVirtualTestROFolder
        {
            get
            {
                var result = LinkitConfigurationManager.GetS3Settings().AUVirtualTestROFolder;
                return result;
            }
        }

        public string AUVirtualTestFolder
        {
            get
            {
                var result = LinkitConfigurationManager.GetS3Settings().AUVirtualTestFolder;
                return result;
            }
        }

        public string UpLoadBucketName
        {
            get
            {
                var result = LinkitConfigurationManager.GetS3Settings().AUVirtualTestBucketName;
                return result;
            }
        }

        public string DownLoadBucketName
        {
            get
            {
                var result = LinkitConfigurationManager.GetS3Settings().S3ItemBucketName;
                return result;
            }
        }

        public string S3Domain
        {
            get
            {
                var result = LinkitConfigurationManager.GetS3Settings().S3Domain;
                return result;
            }
        }

        public bool UseS3Content { get; set; }

        public string TestMediaFolderPath
        {
            get
            {
                var subDomain = UrlUtil.GenerateS3Subdomain(S3Domain, UpLoadBucketName);
                var result = string.Format("{0}/{1}", subDomain.RemoveEndSlash(), AUVirtualTestFolder);
                return result;
            }
        }

        public string PassageMediaFolderPath
        {
            get
            {
                var result = string.Format("{0}{1}{2}", S3Domain, UpLoadBucketName, AUVirtualTestROFolder);
                if (result.EndsWith("/")) result = result.Substring(0, result.Length - 1);
                return result;
            }
        }

        // ############################### VIRTUAL SECTION PROPERTIES ####################################
        public string FileSystemSectionMediaRelativePath
        {
            get
            {
                const string format = "/SectionMedia/Section_{0}";
                var result = string.Format(format, ID);

                return result;
            }
        }

        public string FileSystemQuestionGroupMediaRelativePath
        {
            get
            {
                const string format = "/QuestionGroupMedia/QuestionGroup_{0}";
                var result = string.Format(format, ID);

                return result;
            }
        }

        public string S3SectionMediaRelativePath
        {
            get
            {
                const string format = "SectionMedia/Section_{0}";
                var result = string.Format(format, ID);

                return result;
            }
        }

        public string S3QuestionGroupMediaRelativePath
        {
            get
            {
                const string format = "QuestionGroupMedia/QuestionGroup_{0}";
                var result = string.Format(format, ID);

                return result;
            }
        }

        //public string FileSystemSectionMedia
        //{
        //    get
        //    {
        //        const string mediaPathFormat = "{0}/{1}";
        //        var result = string.Format(mediaPathFormat, FileSystemTestItemMediaPath, FileSystemSectionMediaRelativePath);

        //        return result;
        //    }
        //}

        public string S3SectionMedia
        {
            get
            {
                var result = string.Empty;
                if (string.IsNullOrEmpty(AUVirtualTestFolder))
                {
                    result = S3SectionMediaRelativePath.RemoveStartSlash();
                }
                else
                {
                    result = string.Format("{0}/{1}", AUVirtualTestFolder.RemoveEndSlash(), S3SectionMediaRelativePath.RemoveStartSlash());
                }
                

                return result;
            }
        }

        public string S3QuestionGroupMedia
        {
            get
            {
                var result = string.Empty;
                if (string.IsNullOrEmpty(AUVirtualTestFolder))
                {
                    result = S3QuestionGroupMediaRelativePath.RemoveStartSlash();
                }
                else
                {
                    result = string.Format("{0}/{1}", AUVirtualTestFolder.RemoveEndSlash(), S3QuestionGroupMediaRelativePath.RemoveStartSlash());
                }


                return result;
            }
        }
        // ############################### END OF VIRTUAL SECTION PROPERTIES ####################################

    }
}